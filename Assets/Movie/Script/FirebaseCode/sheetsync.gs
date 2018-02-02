function myFunction()
{

}

var fireBaseUrl_plain = 'https://zoopmoviequiz.firebaseio.com/'
var secret = 'RFM2p5ch9ghP6391Do4UosruXDz8p0TndB9KMYmd'
var rownum_of_columnNames = 0
var arraySeperater = ";;"
var column_not_used_mark = "notused"
var column_extra_info_seperator = "__"
var sync_status_column_index = 2
var sync_status_synced = "synced"
var sync_status_incomplete = "incomplete"
var sync_status_removed = "removed"
var sync_status_to_be_removed = "remove"

function onOpen()
{
    Logger.log('******onOpen  ');
    var ui = SpreadsheetApp.getUi();
    ui.createMenu('My Custom Menu')
        .addItem('Sync All', 'startSyncAll')
        .addItem('Clean this sheet', 'cleanActiveSheet')
        .addItem('Sync this sheet', 'syncActiveSheet')
        .addItem('Sync QuestionModel', 'startSyncQuestionModel')
        .addItem('Sync PackageModel', 'startSyncPackageModel')
        .addToUi();
}

function onEdit(e)
{
    Logger.log('******onEdit  ');
    if (e)
    {
        var range = e.range;
        if (range.getRow() != 1)
        {
            range.setNote('Last modified: ' + new Date());
            Logger.log("row:" + range.getRow);
            var value = range.getValue();
            //var sSheet = SpreadsheetApp.getActiveSpreadsheet().getActiveSheet();
            var sSheet = e.source.getActiveSheet()
          var r = e.source.getActiveRange();
            if (value == sync_status_incomplete)
            {
                sSheet.getRange(range.getRow(), 1, 1, 25).setBackground("Orange")
              //range.setBackground("Orange")
          }
            if (value == sync_status_to_be_removed)
            {
                sSheet.getRange(range.getRow(), 1, 1, 25).setBackgroundRGB(255, 200, 200)
             //range.setBackground("Red")
          }
            else
            {
                //range.setValue("modified__");
                sSheet.getRange(range.getRow(), 1, 1, 25).setBackground("White")
            range.setBackground("White")
          }
        }
    }
}


function cleanActiveSheet()
{
    var sheet = SpreadsheetApp.getActiveSpreadsheet().getActiveSheet();
    var split_sheet_name = sheet.getName().split("_")
  var sheet_name_firstpart = split_sheet_name[0]
  var base = FirebaseApp.getDatabaseByUrl(fireBaseUrl_plain);
    Logger.log("******* cleaned :" + sheet_name_firstpart);
    base.setData("zoopQ/" + sheet_name_firstpart, " ");

    var frozenRows = sheet.getFrozenRows();
    sheet.getRange(frozenRows + 1, 1, sheet.getLastRow(), sheet.getLastColumn()).setBackground(Red)
  sheet.getRange(frozenRows + 1, sync_status_column_index, sheet.getLastRow()).setValue(sync_status_removed)
}

function syncActiveSheet()
{
    var sheet = SpreadsheetApp.getActiveSpreadsheet().getActiveSheet();
    Logger.log('sheet_name:  ' + sheet.getName());
    testWriteDB(sheet)

}

function startSyncQuestionModel()
{
    Logger.log('startSyncQuestionModel');
    var sheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('questions_QuestionModel');
    Logger.log('sheet_name:  ' + sheet.getName());
    /* Testing */
    Logger.log(SpreadsheetApp.getActive().getUrl());
    Logger.log(sheet.getSheetId());
    testWriteDB(sheet)
}

function startSyncPackageModel()
{
    //syncSpreadsheetTest()
    Logger.log('startSyncPackageModel TEST');
    var sheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('PackageModel');
    Logger.log('sheet_name:  ' + sheet.getName());

    //writeDataToFirebase(sheet)
    Test(sheet)
  var dataText = makeJsonQuestionModel(sheet.getDataRange());
    Logger.log(dataText)
  syncSheet(sheet);
}


function testWriteDB(sheet)
{
    Logger.log('sheet_name:  ' + sheet.getName());
    var split_sheet_name = sheet.getName().split("_")
  var sheet_name_firstpart = split_sheet_name[0]
  var column_category_name = "column_category_name";
    var column_name = "column_name";
    var column_if_array = "column_if_array";
    var column_extra_info = "column_extra_info";

    var frozenRows = sheet.getFrozenRows();
    var data = sheet.getDataRange().getValues();

    var arrayOfCategoryName = [];
    var columnInfoAll = { };
    var rowDataOfColumnName = data[rownum_of_columnNames];

    var countColumn = rowDataOfColumnName.length;
    for (var i = 0; i < countColumn; i++)
    {
        var columnInfo = { };
        columnInfo[column_extra_info] = "";
        var arrayNameSplit = rowDataOfColumnName[i].split(column_extra_info_seperator);
        var cName = ""
      if (arrayNameSplit.length > 1)
        {
            columnInfo[column_extra_info] = column_not_used_mark;
        }
        cName = arrayNameSplit[0];
        if (cName != "")
        {
            var s = cName.split("/");
            var restString = ""
          if (s.length > 1)
            {
                columnInfo[column_category_name] = s[0];
                if (arrayOfCategoryName.indexOf(s[0]) == -1)
                {
                    arrayOfCategoryName.push(s[0]);
                }
                restString = s[1];
            }
            else
            {
                columnInfo[column_category_name] = "";
                restString = s[0];
            }
            var s1 = restString.split("_arr");
            columnInfo[column_name] = s1[0];
            if (s1.length > 1)
            {
                columnInfo[column_if_array] = "y"
            }
            else
            {
                columnInfo[column_if_array] = "n"
           }
        }
        else
        {
            columnInfo[column_extra_info] = column_not_used_mark;
            columnInfo[column_name] = cName;
        }
        columnInfoAll[i] = columnInfo;
        Logger.log(i + " => " + columnInfoAll[i][column_category_name] + "     : " + columnInfoAll[i][column_name] + "     : " + columnInfoAll[i][column_if_array]);
    }

    /* Insert Data */
    var base = FirebaseApp.getDatabaseByUrl(fireBaseUrl_plain);
    var countInsert = 0;
    for (var i = frozenRows; i < data.length; i++)
    {
        var status_sync = data[i][1];
        Logger.log("******* status 1 :" + status_sync);
        if (status_sync == sync_status_synced || status_sync == sync_status_incomplete || status_sync == sync_status_removed)
        {
            continue;
        }
        countInsert = countInsert + 1;
        var dataTobeInserted = { };
        for (var c = 0; c < arrayOfCategoryName.length; c++)
        {
            //var catName = arrayOfCategoryName[c]
            dataTobeInserted[arrayOfCategoryName[c]] = { };
        }
        var id = data[i][0];
        var rowDataOfValue = data[i];
        for (var j = 0; j < countColumn; j++)
        {
            var columnInfo = columnInfoAll[j];
            if (columnInfo[column_extra_info] == column_not_used_mark)
            {
                continue;
            }
            var value = rowDataOfValue[j];
            var valueArray = []
          Logger.log("index: " + j);
            Logger.log(value + "  => " + columnInfoAll[j][column_category_name] + "     : " + columnInfoAll[j][column_name] + "     : " + columnInfoAll[j][column_if_array]);

            if (columnInfo[column_category_name] != "")
            {
                var catName = columnInfo[column_category_name];
                if (columnInfo[column_if_array] == "y")
                {
                    valueArray = value.split(arraySeperater);
                    dataTobeInserted[catName][columnInfo[column_name]] = valueArray;
                }
                else
                {
                    dataTobeInserted[catName][columnInfo[column_name]] = value;
                }
            }
            else
            {
                if (columnInfo[column_if_array] == "y")
                {
                    valueArray = value.split(arraySeperater);
                    dataTobeInserted[columnInfo[column_name]] = valueArray;
                }
                else
                {
                    dataTobeInserted[columnInfo[column_name]] = value;
                }
            }

        }


        if (countInsert < 500)
        {
            if (status_sync == sync_status_to_be_removed)
            {
                Logger.log("******* status_2 :" + status_sync + " removed count:" + i);
                base.removeData("zoopQ/" + sheet_name_firstpart + "/" + id)
                 sheet.getRange(i + 1, 1, 1, countColumn).setBackground("Red")
                 sheet.getRange(i + 1, sync_status_column_index).setValue(sync_status_removed);
                sheet.getRange(i + 1, sync_status_column_index).getValue();
            }
            else
            {
                Logger.log("******* status_2:" + status_sync + "   count:" + i);
                base.setData("zoopQ/" + sheet_name_firstpart + "/" + id, dataTobeInserted);
                sheet.getRange(i + 1, 1, 1, countColumn).setBackground("MediumSeaGreen")
          sheet.getRange(i + 1, sync_status_column_index).setValue(sync_status_synced);
                sheet.getRange(i + 1, sync_status_column_index).getValue();
                //sheet.getRange(i, 1, 1, countColumn).setBackgroundRGB(224, 102, 102);
            }

        }
        Logger.log(dataTobeInserted + "\n\n")
    }

    //Logger.log(dataToImport)
    //var base = FirebaseApp.getDatabaseByUrl(fireBaseUrl_plain);
    //exportJSON(dataToImport)
    //syncSpreadsheetTest()
    //base.setData("zoopQ/", dataToImport);
}


/* Utility */
function showData(data)
{
    var app = UiApp.createApplication().setTitle('JSON export results - select all and copy!');
    var textArea = app.createTextArea();
    textArea.setValue(data);
    app.add(textArea);
    textArea.setSize("100%", "100%");
    SpreadsheetApp.getActiveSpreadsheet().show(app);
};

/*Test Function */
function Test(sheet)
{
    var frozenRows = sheet.getFrozenRows();
    var range = sheet.getActiveRange();
    var startRow1 = range.getRowIndex() + frozenRows;
    var startRow = range.getRow();
    var C = range.getColumnIndex();
    var H = range.getHeight();
    var W = range.getWidth()
  var A1not = range.getA1Notation();
    if (H < 2 && W < 2)
    {
        Browser.msgBox('frozenRows=' + frozenRows + '/ Not enough Row = ' + startRow + ' / Col = ' + C + ' Width = ' + W + '  / Heigth = ' + H + '  A1 notation = ' + A1not);
    }
    else
    {
        var data = sheet.getRange(startRow, 1, H, W).getValues();//(StartRow,StartColumn,NumberofRowstoGet,NumberofColumnstoGet)
        Browser.msgBox('frozenRows=' + frozenRows + '/ Row = ' + startRow + ' / Col = ' + C + ' Width = ' + W + '  / Heigth = ' + H + '  A1 notation = ' + A1not);
    }
}

function syncSpreadsheetTest()
{
    //Get the currently active sheet
    var sheet = SpreadsheetApp.getActiveSheet();
    //Get the number of rows and columns which contain some content
    var[rows, columns] = [sheet.getLastRow(), sheet.getLastColumn()];
    //Get the data contained in those rows and columns as a 2 dimensional array
    var data = sheet.getRange(1, 1, rows, columns).getValues();
    //Use the syncMasterSheet function defined before to push this data to the "masterSheet" key in the firebase database
    //var fireBaseUrl_plain1 = 'https://unityfirebasetest-5c0bc.firebaseio.com/'
    var fireBaseUrl_plain1 = 'https://zoopmoviequiz.firebaseio.com/'
  var base = FirebaseApp.getDatabaseByUrl(fireBaseUrl_plain1);//(fireBaseUrl_plain,secret);
    Logger.log(data)
  showData("testtttt");
    base.setData(" ", "tes11111t");
}


