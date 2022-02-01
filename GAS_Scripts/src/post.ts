import { Consts } from "./consts";
import { Utils } from "./utils";

// GAS's event function that will be called when https POST is requested.
function doPost(e: any) {
  const request = JSON.parse(e.postData.getDataAsString());

  if (request == null) {
    return ContentService.createTextOutput("Error: payload was empty.");
  }

  if (request[Consts.CONSTS.Method] == Consts.CONSTS.SaveDatasMethod) {
    return saveDatas(request);
  } else if (request[Consts.CONSTS.Method] == Consts.CONSTS.RemoveDataMethod) {
    return removeData(request);
  } else if (
    request[Consts.CONSTS.Method] == Consts.CONSTS.RemoveUserDatasMethod
  ) {
    return removeUserDatas(request);
  }
}

// function doPost(e: any): GoogleAppsScript.Content.TextOutput {
//   const result = executeDoPost(e);
//   return ContentService.createTextOutput(JSON.stringify(result));
// }

// function executeDoPost(e: any) {
//   console.log("start executeDoPost");
//   return { status: "ok", method: "post" };
// }

function saveDatas(request: any) {
  const currentTime = Utilities.formatDate(
    new Date(),
    "GMT+9",
    "yyyy/MM/dd HH:mm:ss"
  );

  const gssUrl = request[Consts.CONSTS.GssUrl];
  const dataListSheet = Utils.getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = Utils.getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[Consts.CONSTS.UserName];

  let userId = Utils.findUserId(userListSheetData, userName);
  const payloadDatas = request[Consts.CONSTS.Datas];

  if (userId == null) {
    userId = Utils.findMaxUserId(userListSheetData) + 1;
    let addingUser = [];
    addingUser[Consts.CONSTS.UserIdColumn] = userId;
    addingUser[Consts.CONSTS.UserNameColumn] = userName;
    addingUser[Consts.CONSTS.UpdateTimeColumn] = currentTime;
    userListSheet.appendRow(addingUser);
  }

  const userRows = Utils.findUserRowsByUserId(dataListSheetData, userId);

  for (const data of payloadDatas) {
    let addingData = [];
    addingData[Consts.CONSTS.UserIdColumn] = userId;
    let dataString = JSON.stringify(data);

    let dataExisted = false;
    for (let userRow of userRows) {
      if (dataListSheetData[userRow][Consts.CONSTS.DataColumn] == dataString) {
        dataListSheet
          .getRange(userRow + 1, Consts.CONSTS.UpdateTimeColumn + 1)
          .setValue(currentTime);
        dataExisted = true;
        break;
      }
    }

    if (!dataExisted) {
      addingData[Consts.CONSTS.DataColumn] = dataString;
      addingData[Consts.CONSTS.UpdateTimeColumn] = currentTime;
      dataListSheet.appendRow(addingData);
    }
  }

  return ContentService.createTextOutput("Saving datas succeeded.");
}

function removeData(request) {
  const gssUrl = request[Consts.CONSTS.GssUrl];
  const dataListSheet = Utils.getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = Utils.getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[Consts.CONSTS.UserName];
  const userId = Utils.findUserId(userListSheetData, userName);
  const data = JSON.stringify(request[Consts.CONSTS.Data]);

  if (userId != null) {
    const userRows = Utils.findUserRowsByUserId(dataListSheetData, userId);

    userRows.reverse();
    for (let userRow of userRows) {
      if (dataListSheetData[userRow][Consts.CONSTS.DataColumn] == data) {
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
  const gssUrl = request[Consts.CONSTS.GssUrl];
  const dataListSheet = Utils.getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = Utils.getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[Consts.CONSTS.UserName];
  const userId = Utils.findUserId(userListSheetData, userName);

  if (userId != null) {
    const userRows = Utils.findUserRowsByUserId(dataListSheetData, userId);

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
