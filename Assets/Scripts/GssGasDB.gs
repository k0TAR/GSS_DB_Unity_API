var sheetName = 'sheet';

// GETメソッドで実行される
function doGet(e){
  var gssKey = '1eyClP-LRNuCNAMIhdG2CcgjbYJsuOAMAfXOoTvEMFTk';
  var gssSheet = SpreadsheetApp.openById(gssKey).getSheetByName(sheetName);
  var keys = ['userId', 'updateTime', 'playerName', 'message'];
  var userId = '001';
  if(gssSheet == null) {
    return ContentService.createTextOutput("Error: Invalid sheet name");
  }

  var sheetData = gssSheet.getDataRange().getValues();
  var header = sheetData[0]
  
  // Find whether user id already exists
  var row = findRowByUserId(sheetData, userId);

  if(row == 0){
    return ContentService.createTextOutput("Error: Invalid user id");
  }
  
  var result = [];
  for(var i = 0; i < keys.length; i++){
    var column = findColumnByKey(header, keys[i]);
    result.push(column == 0 ? "#NULL#" : sheetData[row][column]);
  }
  
  var j = ContentService.createTextOutput(JSON.stringify(result));
  Logger.log(j.getContent());
  return j;
}

function findRowByUserId(sheetData, userId) {
  var row = 0;
  for(var i = 1; i < sheetData.length; i++){
    if(sheetData[i][0] == userId){
      row = i;
      //finding the first row and end.
      break;
    }
  }
  return row;
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