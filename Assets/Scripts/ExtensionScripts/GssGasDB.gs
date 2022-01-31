const GssUrl =
  "For testing in GAS, type in the GSS URL that you are going to use.";
// Constants used and shared within GAS and Unity.
const CONSTS = {
  Payloads: "payloadDatas",
  UserId: "userId", //An id that will probably always exist in any data design.
  UserName: "userName",
  Data: "data",
  Datas: "datas",
  Time: "updateTime",
  GssUrl: "gssUrl",
  Method: "method",
  SaveDatasMethod: "SaveDatas",
  RemoveDataMethod: "RemoveData",
  RemoveUserDatasMethod: "RemoveUserDatas",
  GetAllDatasMethod: "GetAllDatas",
  GetUserDatasMethod: "GetUserDatas",
  GetUserNamesMethod: "GetUserNames",
  CheckIfGssUrlValidMethod: "CheckIfGssUrlValid",
  CheckIfGasUrlValidMethod: "CheckIfGasUrlValid",
  UpdateTimeColumn: 2,
  UserIdColumn: 0,
  UserNameColumn: 1,
  DataColumn: 1,
  IsClosed: "isClosed",
  AreaId: "areaId",
  VertexId: "vertexId",
  Position: "position",
};

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

function getAllDatas(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const userListSheetData = getUserListSheet(gssUrl).getDataRange().getValues();

  const dataListSheetData = getDataListSheet(gssUrl).getDataRange().getValues();
  dataListSheetData.splice(0, 1);

  let returning_datas = {};
  let datas = [];
  for (let i = 0; i < dataListSheetData.length; i++) {
    data = new Object();
    data[CONSTS.UserName] = findUserByUserId(
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
  const userListSheetData = getUserListSheet(gssUrl).getDataRange().getValues();

  userIdsData = {
    [CONSTS.Payloads]: findUserNames(userListSheetData),
  };
  sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(userIdsData)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function isGssUrlValid(request) {
  const gssUrl = request[CONSTS.GssUrl];

  try {
    getDataListSheet(gssUrl);
    const log = `GssUrl is valid.`;
    return ContentService.createTextOutput(log);
  } catch (e) {
    return ContentService.createTextOutput(e);
  }
}

function isGasUrlValid() {
  return ContentService.createTextOutput(`GasUrl is valid.`);
}

// GAS's event function that will be called when https GET is requested.
function doGet(e) {
  //e == null is just for debugging in GAS.
  //doesn't really matter with the actual function of codes.
  const request = e == null ? generateDebugObjectForGET() : e.parameter;

  if (request == null) {
    Logger.log("Error: payload was empty.");
    return ContentService.createTextOutput("Error: payload was empty.");
  }

  if (request[CONSTS.Method] == CONSTS.GetAllDatasMethod) {
    return getAllDatas(request);
  } else if (request[CONSTS.Method] == CONSTS.GetUserNamesMethod) {
    return getUserNames(request);
  } else if (request[CONSTS.Method] == CONSTS.CheckIfGssUrlValidMethod) {
    return isGssUrlValid(request);
  } else if (request[CONSTS.Method] == CONSTS.CheckIfGasUrlValidMethod) {
    return isGasUrlValid();
  } else {
    return ContentService.createTextOutput(
      `Error: \"${CONSTS.Method}\" is invalid.`
    );
  }
}

function generateDebugObjectForGET() {
  //[variable], [] makes the variable to expand.
  const fakePayload = {
    [CONSTS.Method]: [CONSTS.GetUserNamesMethod],
    [CONSTS.UserName]: "tester",
    [CONSTS.GssUrl]: GssUrl,
  };
  return fakePayload;
}

function saveDatas(request) {
  const currentTime = Utilities.formatDate(
    new Date(),
    "GMT+9",
    "yyyy/MM/dd HH:mm:ss"
  );

  const gssUrl = request[CONSTS.GssUrl];
  const dataListSheet = getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];

  let userId = findUserId(userListSheetData, userName);
  const payloadDatas = request[CONSTS.Datas];

  if (userId == null) {
    userId = findMaxUserId(userListSheetData) + 1;
    let addingUser = [];
    addingUser[CONSTS.UserIdColumn] = userId;
    addingUser[CONSTS.UserNameColumn] = userName;
    addingUser[CONSTS.UpdateTimeColumn] = currentTime;
    userListSheet.appendRow(addingUser);
  }

  const userRows = findUserRowsByUserId(dataListSheetData, userId);

  for (const data of payloadDatas) {
    let addingData = [];
    addingData[CONSTS.UserIdColumn] = userId;
    dataString = JSON.stringify(data);

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
  const dataListSheet = getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = findUserId(userListSheetData, userName);
  const data = JSON.stringify(request[CONSTS.Data]);

  if (userId != null) {
    const userRows = findUserRowsByUserId(dataListSheetData, userId);

    userRows.reverse();
    for (let userRow of userRows) {
      if (dataListSheetData[userRow][CONSTS.DataColumn] == data) {
        dataListSheet.deleteRows(1 + Number(userRow));
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
  const dataListSheet = getDataListSheet(gssUrl);
  const dataListSheetData = dataListSheet.getDataRange().getValues();
  const userListSheet = getUserListSheet(gssUrl);
  const userListSheetData = userListSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = findUserId(userListSheetData, userName);

  if (userId != null) {
    const userRows = findUserRowsByUserId(dataListSheetData, userId);

    userRows.reverse();
    for (let userRow of userRows) {
      dataListSheet.deleteRows(1 + Number(userRow));
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

function findMaxAreaId(sheetData) {
  let maxAreaId = 0;
  for (let i = 1; i < sheetData.length; i++) {
    if (
      maxAreaId <
      JSON.parse(sheetData[userRow][CONSTS.DataColumn])[CONSTS.AreaId]
    ) {
      maxAreaId = sheetData[i][CONSTS.UserIdColumn];
    }
  }
  return maxAreaId;
}

// GAS's event function that will be called when https POST is requested.
function doPost(e) {
  const request =
    e == null
      ? generateDebugObjectForPOST()
      : JSON.parse(e.postData.getDataAsString());

  if (request == null) {
    return ContentService.createTextOutput("Error: payload was empty.");
  }

  if (request[CONSTS.Method] == CONSTS.SaveDatasMethod) {
    return saveDatas(request);
  } else if (request[CONSTS.Method] == CONSTS.RemoveDataMethod) {
    return removeData(request);
  } else if (request[CONSTS.Method] == CONSTS.RemoveUserDatasMethod) {
    return removeUserDatas(request);
  }
}

function generateDebugObjectForPOST() {
  const fakePayload = {
    [CONSTS.Method]: [CONSTS.SaveDatasMethod],
    [CONSTS.GssUrl]: GssUrl,
    [CONSTS.UserName]: "tester",
    [CONSTS.Data]: { areaId: 0, vertexId: 0, position: { x: 0, y: 0, z: 0 } },
    [CONSTS.Datas]: [
      { areaId: 0, vertexId: 0, position: { x: 4, y: 2, z: 0 } },
      { areaId: 0, vertexId: 1, position: { x: 4, y: 4, z: 0 } },
      { areaId: 0, vertexId: 2, position: { x: 2, y: 4, z: 0 } },
      { areaId: 0, vertexId: 3, position: { x: 2, y: 2, z: 0 } },
      { areaId: 0, vertexId: 4, position: { x: 4, y: 2, z: 0 } },
    ],
  };
  return fakePayload;
}
