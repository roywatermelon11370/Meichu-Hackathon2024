/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 * See LICENSE in the project root for license information.
 */

/* global console, document, Excel, Office */

Office.onReady((info) => {
  if (info.host === Office.HostType.Excel) {
    // Assign event handlers and other initialization logic.
    //document.getElementById("create-table").onclick = () => tryCatch(createTable);
    //change_select();
    const socket = new WebSocket("ws://localhost:8880");
    socket.onopen = function(){
      console.log("connected to plugin");
    }
    change_select(socket);
  }
});
async function change_select(ws) {
  await Excel.run(function (context) {
    var sheet = context.workbook.worksheets.getActiveWorksheet();

    // Add an event handler to listen for selection changes.
    sheet.onSelectionChanged.add(function (eventArgs) {
      // Log the address of the selected cell.
      let s = "Selected range: " + eventArgs.address;
      document.getElementById("message").innerText = s;
      ws.send(s);
    });

    return context.sync();
  });
}

async function workbook_select() {
  await Excel.run(function (context) {
    var sheet = context.workbook();

    // Add an event handler to listen for selection changes.
    sheet.onSelectionChanged.add(function (eventArgs) {
      // Log the address of the selected cell.
      document.getElementById("message").innerText = "Selected range: " + eventArgs.address;
    });

    return context.sync();
  });
}
/** Default helper for invoking an action and handling errors. */
async function tryCatch(callback) {
  try {
    await callback();
  } catch (error) {
    // Note: In a production add-in, you'd want to notify the user through your add-in's UI.
    console.error(error);
  }
}
