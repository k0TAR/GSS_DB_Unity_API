const gssKey = '1blYnOjyN0IFr8IPFjjzfXd7AtivAzxZR2qhI00vlBUE';
const sheetName = 'sheet';
// const keys = ['userId', 'updateTime', 'playerName', 'message'];
// Constants used and shared within GAS and Unity.
const PAYLOAD_CONSTS = {
  Method : "method",
  Payload: "payload",
  UserId: "userId",
  SaveDataMethod: "saveData",
  GetDatasMethod: "getUserDatas",
  GetUserIdMethod: "getUserIds"
};

function getUserIds(){
  let gssSheet = SpreadsheetApp.openById(gssKey).getSheetByName(sheetName);
  if(gssSheet == null) {
    return ContentService.createTextOutput("Error: Invalid sheet name.");
  }
  const sheetData = gssSheet.getDataRange().getValues();

  userIdsData = {
    [PAYLOAD_CONSTS.Payload] : findUserIds(sheetData)
  }
  sendingBackPayload = ContentService.createTextOutput(JSON.stringify(userIdsData));
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function findUserIds(sheetData)
{
  let userIdsSet = new Set();
  const userIdColumn = findUserIdColumn(sheetData[0]);
  //from 1 to skip the header row.
  for(let i = 1; i < sheetData.length; i++){
    userIdsSet.add(sheetData[i][userIdColumn]);
  }
  //Spread syntax: expanding
  let userIdsArr = [];
  for(let userId of userIdsSet)
  {
    let userIdElement = new Object();
    userIdElement[PAYLOAD_CONSTS.UserId] = userId;
    userIdsArr.push(userIdElement);
  }
  return userIdsArr;
}

function findUserIdColumn(sheetHeader){
  for(let i = 0; i < sheetHeader.length; i++){
    if(sheetHeader[i] == PAYLOAD_CONSTS.UserId){
      return i;
    }
  }
}

function getUserDatas(userId){
  let gssSheet = SpreadsheetApp.openById(gssKey).getSheetByName(sheetName);
  if(gssSheet == null) {
    return ContentService.createTextOutput("Error: Invalid sheet name.");
  }

  const sheetData = gssSheet.getDataRange().getValues();
  const sheetHeader = sheetData[0];
  
  // Find whether user id already exists
  const user_rows = findUserRowsByUserId(sheetData, userId);

  if(user_rows.length == 0){
    return ContentService.createTextOutput(`Error: \"${userId}\" was invalid userId.`);
  }

  let returning_datas = {};
  let datas = [];
  for(let i = 0; i < user_rows.length; i++)
  {
    data = new Object();
    for(let j = 2; j < sheetHeader.length; j++){
      data[sheetHeader[j]]= sheetData[user_rows[i]][j] ;
    }
    datas[i] = data;
  }
  returning_datas[PAYLOAD_CONSTS.Payload] = datas;

  const sendingBackPayload = ContentService.createTextOutput(JSON.stringify(returning_datas));
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function generateDebugTestPayloadObject(){
  //[variable], [] makes the variable to expand.
  const fakePayload = {
    [PAYLOAD_CONSTS.Method] : [PAYLOAD_CONSTS.GetUserIdMethod],
    [PAYLOAD_CONSTS.UserId] : "001",
  };
  //fakePayload[PAYLOAD_CONSTS.Method]= PAYLOAD_CONSTS.GetDatasMethod;
  //fakePayload[PAYLOAD_CONSTS.UserId] = "001";
  return fakePayload;
}

// GAS's event function that will be called when https GET is requested.
function doGet(e){
  //e == null is just for debugging in GAS.
  //doesn't really matter with the actual function of codes.
  const request = (e == null) ? generateDebugTestPayloadObject() : e.parameter;

  if(request == null ){
    return ContentService.createTextOutput("Error: payload was empty.");
  }
  
  if(request[PAYLOAD_CONSTS.Method] == PAYLOAD_CONSTS.GetDatasMethod){
    var userId = request["userId"];
    return getUserDatas(userId);
  }
  else if(request[PAYLOAD_CONSTS.Method] == PAYLOAD_CONSTS.GetUserIdMethod){
    return getUserIds();
  }
}

function findUserRowsByUserId(sheetData, userId) {
  let rows = [];
  for(let i = 1; i < sheetData.length; i++){
    if(sheetData[i][0] == userId){
      rows.push(i);
    }
  }
  return rows;
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
  // sheetHeader data
  var sheetHeader = sheetData[0];
  
  // Find whether user id already exists
  var row = findUserRowsByUserId(sheetData, userId);
  if(row != 0){
    // User id already exists
    for(var key in data){
      var value = data[key];
      // Find whether the key of the data to be saved aleardy exists
      var column = findColumnByKey(sheetHeader, key);
      if(column != 0){
        // The key already exists, overwrite the value
        sheet.getRange(row, column).setValue(value);
      }else{
        // Adds a new key column
        sheetHeader.push(key);
        sheet.getRange(1, sheetHeader.length).setValue(key);
        // Appends the value of the key
        sheet.getRange(row, sheetHeader.length).setValue(value);
      }
    }
  }else{
    // Insert sheetHeader title if it has not been set
    if(sheetHeader == null || sheetHeader.length < 2){
      var values = [["userId", "updateTime"]];
      sheet.getRange(1, 1, 1, 2).setValues(values);
      sheetHeader = values[0];
    }
    
    // Appends a new row
    var content = [];
    for(var i = 0; i < sheetHeader.length; i++) content.push("");
    content[0] = userId;
    content[1] = time;
    for(var key in data){
      var value = data[key];
      var column = findColumnByKey(sheetHeader, key);
      if(column != 0){
        content[column - 1] = value;
      }else{
        // Adds a new key column
        sheetHeader.push(key);
        sheet.getRange(1, sheetHeader.length).setValue(key);
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
  var sheetHeader = sheetData[0]
  
  // Find whether user id already exists
  var row = findUserRowsByUserId(sheetData, userId);
  if(row == 0){
    return ContentService.createTextOutput("Error: Invalid user id");
  }
  
  var ret = [];
  for(var i = 0; i < keys.length; i++){
    var column = findColumnByKey(sheetHeader, keys[i]);
    ret.push(column == 0 ? "#NULL#" : sheetData[row - 1][column - 1]);
  }
  
  return ContentService.createTextOutput(JSON.stringify(ret));
}

function findUserRowsByUserId(sheetData, userId) {
  var row = 0;
  for(var i = 1; i < sheetData.length; i++){
    if(sheetData[i][0] == userId){
      row = i + 1;
      break;
    }
  }
  return row;
}

function findColumnByKey(sheetHeader, key) {
  var column = 0;
  for(var i = 2; i < sheetHeader.length; i++){
    if(sheetHeader[i] == key){
      column  = i + 1;
      break;
    }
  }
  return column;
}
*/