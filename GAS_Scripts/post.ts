//eslint-disable-next-line @typescript-eslint/no-unused-vars
function doGet(e: Record<string, unknown>) {
  const result = executeDoGet(e);
  return converObjectToJsonString(result);
}

//eslint-disable-next-line @typescript-eslint/no-unused-vars
function doPost(e: Record<string, unknown>) {
  const result = executeDoPost(e);
  return converObjectToJsonString(result);
}

function executeDoGet(e: Record<string, unknown>) {
  console.log("start executeDoGet");
  return { status: "ok", method: "get" };
}

function executeDoPost(e: Record<string, unknown>) {
  console.log("start executeDoPost");
  return { status: "ok", method: "post" };
}

function converObjectToJsonString(result: Record<string, unknown>) {
  const payload = ContentService.createTextOutput(
    JSON.stringify(result)
  ).setMimeType(ContentService.MimeType.JSON);
  return payload;
}
