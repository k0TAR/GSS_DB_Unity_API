const GssUrl =
  "For testing in GAS, type in the GSS URL that you are going to use.";
// Constants used and shared within GAS and Unity.
const CONSTS = {
  Payload: "actualPayload",
  UserId: "userId", //An id that will probably always exist in any data design.
  UserName: "userName",
  Message: "message",
  Time: "updateTime",
  GssUrl: "gssUrl",
  Method: "method",
  SaveDataMethod: "SaveData",
  UpdateDatasMethod: "UpdateDatas",
  RemoveDataMethod: "RemoveData",
  RemoveAreaMethod: "RemoveArea",
  GetAllDatasMethod: "GetAllDatas",
  SetUserDataMethod: "SetUserData",
  GetUserDatasMethod: "GetUserDatas",
  GetUserNamesMethod: "GetUserNames",
  CheckIfGssUrlValidMethod: "CheckIfGssUrlValid",
  CheckIfGasUrlValidMethod: "CheckIfGasUrlValid",
  CheckIfPlayerNameValidMethod: "CheckIfPlayerNameValid",
  UpdateTimeColumn: 0,
  UserIdColumn: 1,
  UserNameColumn: 2,
  MessageColumn: 3,
  IsClosed: "isClosed",
  AreaId: "areaId",
  VertexId: "vertexId",
  Position: "position",
};

function getPositionsSheet(gssUrl) {
  try {
    const gssSheet = SpreadsheetApp.openByUrl(gssUrl);
    return gssSheet.getSheetByName("Positions");
  } catch (e) {
    throw new Error(`GssUrl \"${gssUrl}\" is not valid.`);
  }
}

function getUserDataSheet(gssUrl) {
  try {
    const gssSheet = SpreadsheetApp.openByUrl(gssUrl);
    return gssSheet.getSheetByName("UserDatas");
  } catch (e) {
    throw new Error(`GssUrl \"${gssUrl}\" is not valid.`);
  }
}

function findUserNames(sheetData) {
  let userNamesSet = new Set();
  //from 1 to skip the header row.
  for (let i = 1; i < sheetData.length; i++) {
    userNamesSet.add(sheetData[i][CONSTS.UserNameColumn]);
  }

  let userNamesArray = [];
  for (let userName of userNamesSet) {
    let userIdElement = new Object();
    userIdElement[CONSTS.UserName] = userName;
    userNamesArray.push(userIdElement);
  }
  return userNamesArray;
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
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();
  const sheetHeader = sheetData[0];
  sheetData.splice(0, 1);

  let returning_datas = {};
  let datas = [];
  for (let i = 0; i < sheetData.length; i++) {
    data = new Object();
    for (let j = CONSTS.UserNameColumn; j < sheetHeader.length; j++) {
      data[sheetHeader[j]] = sheetData[i][j];
    }
    datas[i] = data;
  }
  returning_datas[CONSTS.Payload] = datas;

  const sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(returning_datas)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function getUserNames(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  userIdsData = {
    [CONSTS.Payload]: findUserNames(sheetData),
  };
  sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(userIdsData)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function getUserDatas(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getUserDataSheet(gssUrl);
  const positionsSheetData = getPositionsSheet(gssUrl)
    .getDataRange()
    .getValues();
  const sheetData = gssSheet.getDataRange().getValues();
  const sheetHeader = positionsSheetData[0];
  sheetData.splice(0, 1);

  let returning_datas = {};
  let datas = [];
  for (let i = 0; i < sheetData.length; i++) {
    data = new Object();
    for (let j = CONSTS.UserNameColumn; j < sheetHeader.length; j++) {
      data[sheetHeader[j]] = sheetData[i][j];
    }
    datas[i] = data;
  }
  returning_datas[CONSTS.Payload] = datas;

  const sendingBackPayload = ContentService.createTextOutput(
    JSON.stringify(returning_datas)
  );
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function isGssUrlValid(request) {
  const gssUrl = request[CONSTS.GssUrl];

  try {
    getPositionsSheet(gssUrl);
    const log = `GssUrl is valid.`;
    return ContentService.createTextOutput(log);
  } catch (e) {
    return ContentService.createTextOutput(e);
  }
}

function isGasUrlValid() {
  return ContentService.createTextOutput(`GasUrl is valid.`);
}

function isPlayerNameValid(request) {
  const playerName = request[CONSTS.UserName];

  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  const userNames = findUserNames(sheetData);

  const playerNameValid =
    userNames.find((e) => {
      return e[CONSTS.UserName] == playerName;
    }) == null;
  const log = playerNameValid
    ? `PlayerName: ${playerName} is Valid.`
    : `PlayerName: ${playerName} is Invalid.`;
  Logger.log(log);
  sendingBackPayload = ContentService.createTextOutput(log);
  return sendingBackPayload;
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
  } else if (request[CONSTS.Method] == CONSTS.GetUserDatasMethod) {
    return getUserDatas(request);
  } else if (request[CONSTS.Method] == CONSTS.GetUserNamesMethod) {
    return getUserNames(request);
  } else if (request[CONSTS.Method] == CONSTS.CheckIfGssUrlValidMethod) {
    return isGssUrlValid(request);
  } else if (request[CONSTS.Method] == CONSTS.CheckIfGasUrlValidMethod) {
    return isGasUrlValid();
  } else if (request[CONSTS.Method] == CONSTS.CheckIfPlayerNameValidMethod) {
    return isPlayerNameValid(request);
  } else {
    return ContentService.createTextOutput(
      `Error: \"${CONSTS.Method}\" is invalid.`
    );
  }
}

function generateDebugObjectForGET() {
  //[variable], [] makes the variable to expand.
  const fakePayload = {
    [CONSTS.Method]: [CONSTS.GetUserDatasMethod],
    [CONSTS.UserName]: "tester",
    [CONSTS.GssUrl]: GssUrl,
  };
  return fakePayload;
}

function saveData(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  const currentTime = Utilities.formatDate(
    new Date(),
    "GMT+9",
    "yyyy/MM/dd HH:mm:ss"
  );

  const userId = findUserId(
    getUserDataSheet(gssUrl).getDataRange().getValues(),
    request[CONSTS.UserName]
  );
  const messageObj = request[CONSTS.Message];
  const areaId = messageObj[CONSTS.AreaId];
  const vertexId = messageObj[CONSTS.VertexId];
  const inputtingMessage = JSON.stringify(messageObj);

  if (userId != null) {
    const userRows = findUserRowsByUserId(sheetData, userId);

    for (let userRow of userRows) {
      const gssMessageObj = JSON.parse(
        sheetData[userRow][CONSTS.MessageColumn]
      );

      if (gssMessageObj[CONSTS.AreaId] == areaId) {
        if (gssMessageObj[CONSTS.VertexId] == vertexId) {
          gssSheet
            .getRange(1 + Number(userRow), 1 + CONSTS.UpdateTimeColumn)
            .setValue(currentTime);
          gssSheet
            .getRange(1 + Number(userRow), 1 + CONSTS.MessageColumn)
            .setValue(inputtingMessage);
          return ContentService.createTextOutput(
            `areaId and vertexId already existed for userId. Updated position.`
          );
        }
      }
    }
  }

  let addingData = [];
  addingData[CONSTS.UserIdColumn] =
    userId == null
      ? findMaxUserId(getUserDataSheet(gssUrl).getDataRange().getValues()) + 1
      : userId;
  addingData[CONSTS.UpdateTimeColumn] = currentTime;
  addingData[CONSTS.UserNameColumn] = request[CONSTS.UserName];
  addingData[CONSTS.MessageColumn] = inputtingMessage;
  gssSheet.appendRow(addingData);

  return ContentService.createTextOutput("Save data succeeded.");
}

function updateDatas(request) {
  const supposedPayload = {
    [CONSTS.Method]: [CONSTS.UpdateDatasMethod],
    [CONSTS.GssUrl]: GssUrl,
    [CONSTS.UserName]: "tester3",
    [CONSTS.Message]: [
      { areaId: 1, vertexId: 0, position: { x: 4, y: 2, z: 0 } },
      { areaId: 1, vertexId: 1, position: { x: 4, y: 4, z: 0 } },
      { areaId: 1, vertexId: 2, position: { x: 2, y: 4, z: 0 } },
      { areaId: 1, vertexId: 3, position: { x: 2, y: 2, z: 0 } },
      { areaId: 1, vertexId: 4, position: { x: 4, y: 2, z: 0 } },
    ],
  };

  const currentTime = Utilities.formatDate(
    new Date(),
    "GMT+9",
    "yyyy/MM/dd HH:mm:ss"
  );

  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  const userId = findUserId(
    getUserDataSheet(gssUrl).getDataRange().getValues(),
    request[CONSTS.UserName]
  );
  const messageDatas = request[CONSTS.Message];

  if (userId != null) {
    const userRows = findUserRowsByUserId(sheetData, userId);
    const areaId = messageDatas[0][CONSTS.AreaId];

    userRows.reverse();
    for (let userRow of userRows) {
      const gssMessageObj = JSON.parse(
        sheetData[userRow][CONSTS.MessageColumn]
      );
      if (gssMessageObj[CONSTS.AreaId] == areaId) {
        gssSheet.deleteRows(1 + Number(userRow));
      }
    }
  }

  for (const data of messageDatas) {
    let addingData = [];
    addingData[CONSTS.UserIdColumn] =
      userId == null
        ? findMaxUserId(getUserDataSheet(gssUrl).getDataRange().getValues()) + 1
        : userId;
    addingData[CONSTS.UpdateTimeColumn] = currentTime;
    addingData[CONSTS.UserNameColumn] = request[CONSTS.UserName];
    addingData[CONSTS.MessageColumn] = JSON.stringify(data);
    gssSheet.appendRow(addingData);
  }

  return ContentService.createTextOutput("Updating datas succeeded.");
}

function removeData(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = findUserId(
    getUserDataSheet(gssUrl).getDataRange().getValues(),
    userName
  );
  const areaId = request[CONSTS.Message][CONSTS.AreaId];
  const vertexId = request[CONSTS.Message][CONSTS.VertexId];

  if (userId != null) {
    const userRows = findUserRowsByUserId(sheetData, userId);
    for (let userRow of userRows) {
      if (
        JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.AreaId] ==
        areaId
      ) {
        if (
          JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[
            CONSTS.VertexId
          ] == vertexId
        ) {
          gssSheet.deleteRows(1 + Number(userRow));
          return ContentService.createTextOutput(
            `Removed userName=\"${userName}\", areaId=\"${areaId}\", vertexId=\"${vertexId}\" succeeded.`
          );
        }
      }
    }
  }

  const errorLog = `Error : userName=\"${userName}\", areaId=\"${areaId}\", vertexId=\"${vertexId}\" does not exist.`;
  Logger.log(errorLog);
  return ContentService.createTextOutput(errorLog);
}

function removeArea(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getPositionsSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = findUserId(
    getUserDataSheet(gssUrl).getDataRange().getValues(),
    userName
  );
  const messageDatas = request[CONSTS.Message];

  if (userId != null) {
    const userRows = findUserRowsByUserId(sheetData, userId);
    const areaId = messageDatas[CONSTS.AreaId];

    userRows.reverse();
    for (let userRow of userRows) {
      const gssMessageObj = JSON.parse(
        sheetData[userRow][CONSTS.MessageColumn]
      );
      if (gssMessageObj[CONSTS.AreaId] == areaId) {
        gssSheet.deleteRows(1 + Number(userRow));
      }
    }

    return ContentService.createTextOutput(
      `Removed userName=\"${userName}\", areaId=\"${areaId}\" succeeded.`
    );
  } else {
    const errorLog = `Error : userName=\"${userName}\" does not exist.`;
    Logger.log(errorLog);
    return ContentService.createTextOutput(errorLog);
  }
}

function setUserData(request) {
  const gssUrl = request[CONSTS.GssUrl];
  const gssSheet = getUserDataSheet(gssUrl);
  const sheetData = gssSheet.getDataRange().getValues();

  const currentTime = Utilities.formatDate(
    new Date(),
    "GMT+9",
    "yyyy/MM/dd HH:mm:ss"
  );

  const userName = request[CONSTS.UserName];
  const userId = findUserId(sheetData, userName);
  const messageObj = request[CONSTS.Message];
  const inputtingData = JSON.stringify(messageObj);

  if (userId != null) {
    const errorLog = `Error : userName=\"${userName}\" already exists.`;
    Logger.log(errorLog);
    return ContentService.createTextOutput(errorLog);
  }

  let addingData = [];
  addingData[CONSTS.UserIdColumn] =
    userId == null ? findMaxUserId(sheetData) + 1 : userId;
  addingData[CONSTS.UpdateTimeColumn] = currentTime;
  addingData[CONSTS.UserNameColumn] = userName;
  addingData[CONSTS.MessageColumn] = inputtingData;
  gssSheet.appendRow(addingData);

  const successLog = `UserName=\"${userName}\" was entered successfully.`;
  Logger.log(successLog);
  return ContentService.createTextOutput(successLog);
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
      JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.AreaId]
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

  if (request[CONSTS.Method] == CONSTS.SaveDataMethod) {
    return saveData(request);
  } else if (request[CONSTS.Method] == CONSTS.UpdateDatasMethod) {
    return updateDatas(request);
  } else if (request[CONSTS.Method] == CONSTS.RemoveDataMethod) {
    return removeData(request);
  } else if (request[CONSTS.Method] == CONSTS.RemoveAreaMethod) {
    return removeArea(request);
  } else if (request[CONSTS.Method] == CONSTS.SetUserDataMethod) {
    return setUserData(request);
  }
}

function generateDebugObjectForPOST() {
  const fakePayload = {
    [CONSTS.Method]: [CONSTS.SetUserDataMethod],
    [CONSTS.GssUrl]: GssUrl,
    [CONSTS.UserName]: "tester",
    [CONSTS.Message]: [
      { areaId: 0, vertexId: 0, position: { x: 4, y: 2, z: 0 } },
      { areaId: 0, vertexId: 1, position: { x: 4, y: 4, z: 0 } },
      { areaId: 0, vertexId: 2, position: { x: 2, y: 4, z: 0 } },
      { areaId: 0, vertexId: 3, position: { x: 2, y: 2, z: 0 } },
      { areaId: 0, vertexId: 4, position: { x: 4, y: 2, z: 0 } },
    ],
  };
  return fakePayload;
}
