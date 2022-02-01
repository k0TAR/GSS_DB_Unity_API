import { CONSTS } from "./consts";
import * as u from "./utils";

function doPost(e: any): GoogleAppsScript.Content.TextOutput {
  const result = executeDoPost(e);
  return ContentService.createTextOutput(JSON.stringify(result));
}

function executeDoPost(e: any) {
  console.log("start executeDoPost");
  return { status: "ok", method: "post" };
}

function saveDatas(request) {
  const currentTime = Utilities.formatDate(
    new Date(),
    "GMT+9",
    "yyyy/MM/dd HH:mm:ss"
  );

  const gssUrl = request[CONSTS.GssUrl];
  const dataListSheet = u.getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = u.getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];

  let userId = u.findUserId(userListSheetData, userName);
  const payloadDatas = request[CONSTS.Datas];

  if (userId == null) {
    userId = u.findMaxUserId(userListSheetData) + 1;
    let addingUser = [];
    addingUser[CONSTS.UserIdColumn] = userId;
    addingUser[CONSTS.UserNameColumn] = userName;
    addingUser[CONSTS.UpdateTimeColumn] = currentTime;
    userListSheet.appendRow(addingUser);
  }

  const userRows = u.findUserRowsByUserId(dataListSheetData, userId);

  for (const data of payloadDatas) {
    let addingData = [];
    addingData[CONSTS.UserIdColumn] = userId;
    let dataString = JSON.stringify(data);

    let dataExisted = false;
    for (let userRow of userRows) {
      if (dataListSheetData[userRow][CONSTS.DataColumn] == dataString) {
        dataListSheet
          .getRange(userRow + 1, CONSTS.UpdateTimeColumn + 1)
          .setValue(currentTime);
        dataExisted = true;
        break;
      }
    }

    if (!dataExisted) {
      addingData[CONSTS.DataColumn] = dataString;
      addingData[CONSTS.UpdateTimeColumn] = currentTime;
      dataListSheet.appendRow(addingData);
    }
  }

  return ContentService.createTextOutput("Saving datas succeeded.");
}

function removeData(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const dataListSheet = u.getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = u.getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = u.findUserId(userListSheetData, userName);
  const data = JSON.stringify(request[CONSTS.Data]);

  if (userId != null) {
    const userRows = u.findUserRowsByUserId(dataListSheetData, userId);

    userRows.reverse();
    for (let userRow of userRows) {
      if (dataListSheetData[userRow][CONSTS.DataColumn] == data) {
        dataListSheet.deleteRows(1 + Number(userRow), 1);
      }
    }

    return ContentService.createTextOutput(
      `Removing userName=\"${userName}\", data=\"${data}\" succeeded.`
    );
  } else {
    const errorLog = `Error : userName=\"${userName}\", data=\"${data}\" does not exist.`;
    Logger.log(errorLog);
    return ContentService.createTextOutput(errorLog);
  }
}

function removeUserDatas(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const dataListSheet = u.getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = u.getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = u.findUserId(userListSheetData, userName);

  if (userId != null) {
    const userRows = u.findUserRowsByUserId(dataListSheetData, userId);

    userRows.reverse();
    for (let userRow of userRows) {
      dataListSheet.deleteRows(1 + Number(userRow), 1);
    }

    return ContentService.createTextOutput(
      `Removing userName=\"${userName}\"'s datas succeeded.`
    );
  } else {
    const errorLog = `Error : userName=\"${userName}\" does not exist.`;
    Logger.log(errorLog);
    return ContentService.createTextOutput(errorLog);
  }
}
