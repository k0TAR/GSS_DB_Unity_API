var sheetName = 'sheet';


// GETメソッドで実行される
function doGet(e){
  var gssKey = '1blYnOjyN0IFr8IPFjjzfXd7AtivAzxZR2qhI00vlBUE';
  var gssSheet = SpreadsheetApp.openById(gssKey).getSheetByName(sheetName);
  var keys = ['userId', 'updateTime', 'playerName', 'message'];
  var userId = '001';
  if(gssSheet == null) {
    return ContentService.createTextOutput("Error: Invalid sheet name");
  }

  var sheetData = gssSheet.getDataRange().getValues();
  var header = sheetData[0]
  
  // Find whether user id already exists
  var user_rows = findRowsByUserId(sheetData, userId);

  if(user_rows.length == 0){
    return ContentService.createTextOutput("Error: Invalid user id");
  }

  const payloadKey = "payload";
  var returning_datas = {};
  var datas = [];
  for(var i = 0; i < user_rows.length; i++)
  {
    data = new Object();
    for(var j = 0; j < keys.length; j++){
      var column = findColumnByKey(header, keys[j]);
      if(column == 0){
        continue;
      }else{
        data[keys[j]]= sheetData[user_rows[i]][column] ;
      }
    }
    datas[i] = data;
  }
  returning_datas[payloadKey] = datas;

  var j = ContentService.createTextOutput(JSON.stringify(returning_datas));
  Logger.log(j.getContent());
  return j;
}

function findRowsByUserId(sheetData, userId) {
  var rows = [];
  for(var i = 1; i < sheetData.length; i++){
    if(sheetData[i][0] == userId){
      rows.push(i);
    }
  }
  return rows;
}

function findColumnByKey(header, key) {
  var column = 0;
  for(var i = 2; i < header.length; i++){
    if(header[i] == key){
      column  = i;
      break;
    }
  }
  return column;
}

// POSTメソッドで実行される
function doPost(e){

  
  return ContentService.createTextOutput("Save data succeeded");
}

/*
// Constants shared by Unity and this App Script.
var CONST = {
  Method : "method",
  Payload: "payload",
  UserId: "userId",
  SheetName : "sheet",
  Data: "data",
  SaveDataMethod: "saveData",
  GetDataMethod: "getData",
  CellReference: "cellRef"
};

function doPost(e) {
  var request = e.parameter;
  var method = request[CONST.Method];
  
  if(method == CONST.SaveDataMethod){
    return saveData(request[CONST.Payload]);
  }else if(method == CONST.GetDataMethod) {
    return getData(request[CONST.Payload]);
  }
  
  return ContentService.createTextOutput("Error: Invalid method");
}

function saveData(payload) {
  var jsonData = JSON.parse(payload);
  var userId = jsonData[CONST.UserId];
  var sheetName = jsonData[CONST.SheetName];
  var data = jsonData[CONST.Data];
  
  // Created time
  var time = Utilities.formatDate(new Date(), "GMT+9", "yyyy/MM/dd HH:mm:ss");
  
  // Get the sheet
  var spreadSheet = SpreadsheetApp.getActiveSpreadsheet();
  var sheet = spreadSheet.getSheetByName(sheetName);
  if(sheet == null) {
    // Insert sheet if not exists
    sheet = spreadSheet.insertSheet(sheetName);
  }
  
  // Retrieve all data
  var sheetData = sheet.getDataRange().getValues();
  // Header data
  var header = sheetData[0];
  
  // Find whether user id already exists
  var row = findRowsByUserId(sheetData, userId);
  if(row != 0){
    // User id already exists
    for(var key in data){
      var value = data[key];
      // Find whether the key of the data to be saved aleardy exists
      var column = findColumnByKey(header, key);
      if(column != 0){
        // The key already exists, overwrite the value
        sheet.getRange(row, column).setValue(value);
      }else{
        // Adds a new key column
        header.push(key);
        sheet.getRange(1, header.length).setValue(key);
        // Appends the value of the key
        sheet.getRange(row, header.length).setValue(value);
      }
    }
  }else{
    // Insert header title if it has not been set
    if(header == null || header.length < 2){
      var values = [["userId", "updateTime"]];
      sheet.getRange(1, 1, 1, 2).setValues(values);
      header = values[0];
    }
    
    // Appends a new row
    var content = [];
    for(var i = 0; i < header.length; i++) content.push("");
    content[0] = userId;
    content[1] = time;
    for(var key in data){
      var value = data[key];
      var column = findColumnByKey(header, key);
      if(column != 0){
        content[column - 1] = value;
      }else{
        // Adds a new key column
        header.push(key);
        sheet.getRange(1, header.length).setValue(key);
        // Appends the value
        content.push(value);
      }
    }
    sheet.appendRow(content);
  }
  
  return ContentService.createTextOutput("Save data succeeded");
}

function getData(payload) {
  var jsonData = JSON.parse(payload);
  var userId = jsonData[CONST.UserId];
  var sheetName = jsonData[CONST.SheetName];
  var keys = jsonData[CONST.Data];
  var cellRef = jsonData[CONST.CellReference];
  
  var spreadSheet = SpreadsheetApp.getActiveSpreadsheet();
  var sheet = spreadSheet.getSheetByName(sheetName);
  if(sheet == null) {
    return ContentService.createTextOutput("Error: Invalid sheet name");
  }
  
  if(cellRef){
    var values = sheet.getRange(cellRef).getValues();
    var result = []
    for(var i = 0; i < values.length; i++){
      result = result.concat(values[i]);
    }
    return ContentService.createTextOutput(JSON.stringify(result));
  }
  
  var sheetData = sheet.getDataRange().getValues();
  var header = sheetData[0]
  
  // Find whether user id already exists
  var row = findRowsByUserId(sheetData, userId);
  if(row == 0){
    return ContentService.createTextOutput("Error: Invalid user id");
  }
  
  var ret = [];
  for(var i = 0; i < keys.length; i++){
    var column = findColumnByKey(header, keys[i]);
    ret.push(column == 0 ? "#NULL#" : sheetData[row - 1][column - 1]);
  }
  
  return ContentService.createTextOutput(JSON.stringify(ret));
}

function findRowsByUserId(sheetData, userId) {
  var row = 0;
  for(var i = 1; i < sheetData.length; i++){
    if(sheetData[i][0] == userId){
      row = i + 1;
      break;
    }
  }
  return row;
}

function findColumnByKey(header, key) {
  var column = 0;
  for(var i = 2; i < header.length; i++){
    if(header[i] == key){
      column  = i + 1;
      break;
    }
  }
  return column;
}
*/