function handsontableScript(divId, searchId, resultsId, colHeaders, data)
{

        var
          handsontableDiv = document.getElementById(divId),
          handsontableObj, searchField, searchResults,
          searchResultCount = 0;

        function searchResultCounter(instance, row, col, value, result) {
            Handsontable.Search.DEFAULT_CALLBACK.apply(this, arguments);

            if (result)
            {
                searchResultCount++;
            }
        }
  

        handsontableObj = new Handsontable(handsontableDiv,{
          data: data,
        colHeaders: colHeaders,
        height: 400,
        search:
        {
            callback: searchResultCounter
        }
    });
      searchField = document.getElementById(searchId);
      searchResults = document.getElementById(resultsId);

      Handsontable.Dom.addEvent(searchField, 'keyup', function(event)
    {
        var queryResult;

        searchResultCount = 0;
        queryResult = handsontableObj.search.query(this.value);
        var text = '';
        if (searchResultCount > 0)
        {
            text += searchResultCount.toString() + ' results:\r\n';
            var addedRowIndexes = [];
            for (var index = 0; index < queryResult.length; index++)
            {
                var rowIndex = queryResult[index].row;
                if (addedRowIndexes.indexOf(rowIndex) == -1)
                {
                    addedRowIndexes.push(rowIndex);
                    var rowData = data[rowIndex];
                    text += '> { ';
                    for (var columnIndex = 0; columnIndex < rowData.length; columnIndex++)
                    {
                        if (rowData[columnIndex])
                        {
                            text += colHeaders[columnIndex] + '="' + rowData[columnIndex] + '"; ';
                        }
                    }
                    text += '}\r\n';
                }
            }
        }
        searchResults.innerText = text;
        handsontableObj.render();
    });
}