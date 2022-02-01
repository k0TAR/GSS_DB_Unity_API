import { Consts } from "./consts";
import { Utils } from "./utils";

// GAS's event function that will be called when https GET is requested.
function doGet(e) {
  //e == null is just for debugging in GAS.
  //doesn't really matter with the actual function of codes.
  const request = e.parameter;

  if (request == null) {
    Logger.log("Error: payload was empty.");
    return ContentService.createTextOutput("Error: payload was empty.");
  }

  if (request[Consts.CONSTS.Method] == Consts.CONSTS.GetAllDatasMethod) {
    return getAllDatas(request);
  } else if (
    request[Consts.CONSTS.Method] == Consts.CONSTS.GetUserNamesMethod
  ) {
    return getUserNames(request);
  } else if (
    request[Consts.CONSTS.Method] == Consts.CONSTS.CheckIfGssUrlValidMethod
  ) {
    return isGssUrlValid(request);
  } else if (
    request[Consts.CONSTS.Method] == Consts.CONSTS.CheckIfGasUrlValidMethod
  ) {
    return isGasUrlValid();
  } else {
    return ContentService.createTextOutput(
      `Error: \"${Consts.CONSTS.Method}\" is invalid.`
    );
  }
}
// function doGet(e: any): GoogleAppsScript.Content.TextOutput {
//   const result = executeDoGet(e);
//   return ContentService.createTextOutput(JSON.stringify(result));
// }

// function executeDoGet(e: any) {
//   console.log("start executeDoGet");
//   return { status: e.parameter["data"], method: "get" };
// }

function getAllDatas(request) {
  const gssUrl = request[Consts.CONSTS.GssUrl];
  const userListSheetData = Utils.getUserListSheet(gssUrl)
    .getDataRange()
    .getValues();

  const dataListSheetData = Utils.getDataListSheet(gssUrl)
    .getDataRange()
    .getValues();
  dataListSheetData.splice(0, 1);

  let returning_datas = {};
  let datas = [];
  for (let i = 0; i < dataListSheetData.length; i++) {
    let data = new Object();
    data[Consts.CONSTS.UserName] = Utils.findUserByUserId(
      userListSheetData,
      dataListSheetData[i][Consts.CONSTS.UserIdColumn]
    );
    data[Consts.CONSTS.Data] = dataListSheetData[i][Consts.CONSTS.DataColumn];
    datas[i] = data;
  }
  returning_datas[Consts.CONSTS.Payloads] = datas;

  const sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(returning_datas)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function getUserNames(request) {
  const gssUrl = request[Consts.CONSTS.GssUrl];
  const userListSheetData = Utils.getUserListSheet(gssUrl)
    .getDataRange()
    .getValues();

  let userIdsData = {
    [Consts.CONSTS.Payloads]: Utils.findUserNames(userListSheetData),
  };
  let sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(userIdsData)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function isGssUrlValid(request) {
  const gssUrl = request[Consts.CONSTS.GssUrl];

  try {
    Utils.getDataListSheet(gssUrl);
    const log = `GssUrl is valid.`;
    return ContentService.createTextOutput(log);
  } catch (e) {
    return ContentService.createTextOutput(e);
  }
}

function isGasUrlValid() {
  return ContentService.createTextOutput(`GasUrl is valid.`);
}
