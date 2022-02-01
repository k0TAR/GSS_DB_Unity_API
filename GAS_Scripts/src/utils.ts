import { CONSTS } from "./consts";

function getDataListSheet(gssUrl) {
  try {
    const gssSheet = SpreadsheetApp.openByUrl(gssUrl);
    return gssSheet.getSheetByName("DataList");
  } catch (e) {
    throw new Error(`GssUrl \"${gssUrl}\" is not valid.`);
  }
}

function getUserListSheet(gssUrl) {
  try {
    const gssSheet = SpreadsheetApp.openByUrl(gssUrl);
    return gssSheet.getSheetByName("UserList");
  } catch (e) {
    throw new Error(`GssUrl \"${gssUrl}\" is not valid.`);
  }
}

function findUserNames(sheetData) {
  let userNamesSet = new Set();
  //from i = 1 to skip the header row.
  for (let i = 1; i < sheetData.length; i++) {
    userNamesSet.add(sheetData[i][CONSTS.UserNameColumn]);
  }

  let userNamesArray = [];
  for (let userName of userNamesSet) {
    let userNameObject = new Object();
    userNameObject[CONSTS.UserName] = userName;
    userNamesArray.push(userNameObject);
  }
  return userNamesArray;
}

function findUserByUserId(userListSheetData, userId) {
  //from i = 1 to skip the header.
  for (let i = 1; i < userListSheetData.length; i++) {
    if (userListSheetData[i][CONSTS.UserIdColumn] == userId) {
      return userListSheetData[i][CONSTS.UserNameColumn];
    }
  }

  throw new Error(
    `findUserByUserId : userId cannot be found in userListSheetData.`
  );
}

function findUserRowsByUserId(sheetData, userId) {
  let rows = [];
  for (let i = 1; i < sheetData.length; i++) {
    if (sheetData[i][CONSTS.UserIdColumn] == userId) {
      rows.push(i);
    }
  }
  return rows;
}

function findUserId(sheetData, userName) {
  for (let i = 1; i < sheetData.length; i++) {
    if (sheetData[i][CONSTS.UserNameColumn] == userName) {
      return sheetData[i][CONSTS.UserIdColumn];
    }
  }
  return null;
}

function findUserRowsByUserName(sheetData, userName) {
  let rows = [];
  for (let i = 1; i < sheetData.length; i++) {
    if (sheetData[i][CONSTS.UserNameColumn] == userName) {
      rows.push(i);
    }
  }
  return rows;
}

function findMaxUserId(sheetData) {
  let maxId = -1;
  for (let i = 1; i < sheetData.length; i++) {
    if (maxId < sheetData[i][CONSTS.UserIdColumn]) {
      maxId = sheetData[i][CONSTS.UserIdColumn];
    }
  }
  return maxId;
}

export {
  getDataListSheet,
  getUserListSheet,
  findUserNames,
  findUserByUserId,
  findUserRowsByUserId,
  findUserId,
  findUserRowsByUserName,
  findMaxUserId,
};
