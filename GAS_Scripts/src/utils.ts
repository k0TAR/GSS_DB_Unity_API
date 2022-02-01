import { Consts } from "./consts";

export namespace Utils {
  export function getDataListSheet(gssUrl) {
    try {
      const gssSheet = SpreadsheetApp.openByUrl(gssUrl);
      return gssSheet.getSheetByName("DataList");
    } catch (e) {
      throw new Error(`GssUrl \"${gssUrl}\" is not valid.`);
    }
  }

  export function getUserListSheet(gssUrl) {
    try {
      const gssSheet = SpreadsheetApp.openByUrl(gssUrl);
      return gssSheet.getSheetByName("UserList");
    } catch (e) {
      throw new Error(`GssUrl \"${gssUrl}\" is not valid.`);
    }
  }

  export function findUserNames(sheetData) {
    let userNamesSet = new Set();
    //from i = 1 to skip the header row.
    for (let i = 1; i < sheetData.length; i++) {
      userNamesSet.add(sheetData[i][Consts.CONSTS.UserNameColumn]);
    }

    let userNamesArray = [];
    for (let userName of userNamesSet) {
      let userNameObject = new Object();
      userNameObject[Consts.CONSTS.UserName] = userName;
      userNamesArray.push(userNameObject);
    }
    return userNamesArray;
  }

  export function findUserByUserId(userListSheetData, userId) {
    //from i = 1 to skip the header.
    for (let i = 1; i < userListSheetData.length; i++) {
      if (userListSheetData[i][Consts.CONSTS.UserIdColumn] == userId) {
        return userListSheetData[i][Consts.CONSTS.UserNameColumn];
      }
    }

    throw new Error(
      `findUserByUserId : userId cannot be found in userListSheetData.`
    );
  }

  export function findUserRowsByUserId(sheetData, userId) {
    let rows = [];
    for (let i = 1; i < sheetData.length; i++) {
      if (sheetData[i][Consts.CONSTS.UserIdColumn] == userId) {
        rows.push(i);
      }
    }
    return rows;
  }

  export function findUserId(sheetData, userName) {
    for (let i = 1; i < sheetData.length; i++) {
      if (sheetData[i][Consts.CONSTS.UserNameColumn] == userName) {
        return sheetData[i][Consts.CONSTS.UserIdColumn];
      }
    }
    return null;
  }

  export function findUserRowsByUserName(sheetData, userName) {
    let rows = [];
    for (let i = 1; i < sheetData.length; i++) {
      if (sheetData[i][Consts.CONSTS.UserNameColumn] == userName) {
        rows.push(i);
      }
    }
    return rows;
  }

  export function findMaxUserId(sheetData) {
    let maxId = -1;
    for (let i = 1; i < sheetData.length; i++) {
      if (maxId < sheetData[i][Consts.CONSTS.UserIdColumn]) {
        maxId = sheetData[i][Consts.CONSTS.UserIdColumn];
      }
    }
    return maxId;
  }
}
