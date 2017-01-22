function handsontableScript(element, visualizePendingChanges)
{
    // DOM elements:
    var
      handsontableDiv = document.getElementById("handsontable_" + element.typeId),
      searchField = document.getElementById("search_field_" + element.typeId),
      searchResult = document.getElementById("search_result_" + element.typeId);
    
    // Other global variables:
    var handsontableObj;
    var searchResultCount = 0;

    // pending changes contains rowIndex as key, and rowData as value:
    var pendingChanges = new Array();
  
    /**
    * Initializes handsontable
    */
    handsontableObj = new Handsontable(handsontableDiv,{
        data: element.data,
        colHeaders: element.colHeaders,
        height: 400,
        minSpareRows: 1,
        search:
        {
            callback: searchResultCounter
        }
    });

    /**
    * Search
    **/
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
                if (addedRowIndexes.indexOf(rowIndex) === -1)
                {
                    addedRowIndexes.push(rowIndex);
                    var rowData = handsontableObj.getSourceDataAtRow(rowIndex);
                    var rowHtml = element.getRowAsHtmlResultItem(rowData);
                    text += rowHtml;
                }
            }
        }
        searchResult.innerHTML = text;
        handsontableObj.render();
    });

    /**
    * Updates search result counter
    **/
    function searchResultCounter(instance, row, col, value, result) {
        Handsontable.Search.DEFAULT_CALLBACK.apply(this, arguments);
        if (result) {
            searchResultCount++;
        }
    }

    /**
    * On cell change. Local hook (has same effect as a callback).
    * @param changes: 2D array containing information about each of the edited cells [[row, prop, oldVal, newVal], ...]
    **/
    handsontableObj.addHook('afterChange', function (changes, source) {
        var rowIndex = changes[0][0];
        var rowData = handsontableObj.getSourceDataAtRow(rowIndex);
        element.addPendingChange(rowIndex, rowData);
        visualizePendingChanges();
    });

}