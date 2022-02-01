import { CONSTS } from "./consts";
import * as u from "./utils";

function doGet(e: any): GoogleAppsScript.Content.TextOutput {
  const result = executeDoGet(e);
  return ContentService.createTextOutput(JSON.stringify(result));
}

function executeDoGet(e: any) {
  console.log("start executeDoGet");
  return { status: e.parameter["data"], method: "get" };
}

function getAllDatas(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const userListSheetData = u
    .getUserListSheet(gssUrl)
    .getDataRange()
    .getValues();

  const dataListSheetData = u
    .getDataListSheet(gssUrl)
    .getDataRange()
    .getValues();
  dataListSheetData.splice(0, 1);

  let returning_datas = {};
  let datas = [];
  for (let i = 0; i < dataListSheetData.length; i++) {
    let data = new Object();
    data[CONSTS.UserName] = u.findUserByUserId(
      userListSheetData,
      dataListSheetData[i][CONSTS.UserIdColumn]
    );
    data[CONSTS.Data] = dataListSheetData[i][CONSTS.DataColumn];
    datas[i] = data;
  }
  returning_datas[CONSTS.Payloads] = datas;

  const sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(returning_datas)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function getUserNames(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const userListSheetData = u
    .getUserListSheet(gssUrl)
    .getDataRange()
    .getValues();

  let userIdsData = {
    [CONSTS.Payloads]: u.findUserNames(userListSheetData),
  };
  let sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(userIdsData)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function isGssUrlValid(request) {
  const gssUrl = request[CONSTS.GssUrl];

  try {
    u.getDataListSheet(gssUrl);
    const log = `GssUrl is valid.`;
    return ContentService.createTextOutput(log);
  } catch (e) {
    return ContentService.createTextOutput(e);
  }
}

function isGasUrlValid() {
  return ContentService.createTextOutput(`GasUrl is valid.`);
}
