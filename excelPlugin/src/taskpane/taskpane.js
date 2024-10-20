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
    socket.onopen = function () {
      console.log("connected to plugin");
    };
    socket.onmessage = (event) => {
      console.log(event);
      document.getElementById("ws").innerText = "ws:" + event.data;
      if(event.data=="create"){
        createTable();
      }
    };
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
      ws.send("select");
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

async function createTable() {
  await Excel.run(async (context) => {
    // TODO1: Queue table creation logic here.
    const currentWorksheet = context.workbook.worksheets.getActiveWorksheet();
    const expensesTable = currentWorksheet.tables.add("A1:D1", true /*hasHeaders*/);
    expensesTable.name = "ExpensesTable";
    // TODO2: Queue commands to populate the table with data.
    expensesTable.getHeaderRowRange().values = [["Date", "Merchant", "Category", "Amount"]];

    expensesTable.rows.add(null /*add at the end*/, [
      ["1/1/2017", "The Phone Company", "Communications", "120"],
      ["1/2/2017", "Northwind Electric Cars", "Transportation", "142.33"],
      ["1/5/2017", "Best For You Organics Company", "Groceries", "27.9"],
      ["1/10/2017", "Coho Vineyard", "Restaurant", "33"],
      ["1/11/2017", "Bellows College", "Education", "350.1"],
      ["1/15/2017", "Trey Research", "Other", "135"],
      ["1/15/2017", "Best For You Organics Company", "Groceries", "97.88"],
    ]);
    // TODO3: Queue commands to format the table.
    expensesTable.columns.getItemAt(3).getRange().numberFormat = [["\u20AC#,##0.00"]];
    expensesTable.getRange().format.autofitColumns();
    expensesTable.getRange().format.autofitRows();
    await context.sync();
  });
}