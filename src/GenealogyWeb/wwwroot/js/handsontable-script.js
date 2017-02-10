document.addEventListener("DOMContentLoaded", function () {

    var personsKey = 'persons';
    var marriagesKey = 'marriages';
    var sonsKey = 'sons';

    elements.push(new element(personsKey, colHeaders_1, data_1));
    elements.push(new element(marriagesKey, colHeaders_2, data_2));
    elements.push(new element(sonsKey, colHeaders_3, data_3));

    function element(typeId, colHeaders, data) {

        this.typeId = typeId;

        this.colHeaders = colHeaders;
        this.data = data;

        this.getRowAsHtmlResultItem = function (rowIndex, rowData) {

            var text = this.getRowDataAsText(rowData);
            var elementId = rowData[1];

            if (this.typeId == personsKey)
            {
                var onclick1 = "'/data/persondownwardtree?id=" + elementId + "'";
                var link1 = '<a href="#" onclick="return popitup(' + onclick1 + ')">downward tree</a>';

                var onclick2 = "'/data/personupwardtree?id=" + elementId + "'";
                var link2 = '<a href="#" onclick="return popitup(' + onclick2 + ')">upwward tree</a>';

                var onclick3 = "'/data/personById?id=" + elementId + "'";
                var link3 = '<a href="#" onclick="return popitup(' + onclick3 + ')">edit person</a>';

                var onclick4 = "'/data/marriageByPersonId?id=" + elementId + "'";
                var link4 = '<a href="#" onclick="return popitup(' + onclick4 + ')">edit marriage</a>';

                var linkHtml = '<div>' + link1 + ' | ' + link2 + ' | ' + link3 + ' | ' + link4 + '|' + text + '</div>';
                return linkHtml;
            }
            if (this.typeId == marriagesKey)
            {
                var onclick1 = "'/data/marriageById?id=" + elementId + "'";
                var link1 = '<a href="#" onclick="return popitup(' + onclick3 + ')">edit</a>';

                var linkHtml = '<div>' + link1 + '|' + text + '</div>';
                return linkHtml;
            }
            if (this.typeId == sonsKey)
            {

                var onclick1 = "'/data/sonById?id=" + elementId + "'";
                var link1 = '<a href="#" onclick="return popitup(' + onclick3 + ')">edit</a>';

                var linkHtml = '<div>' + link1 + '|' + text + '</div>';
                return linkHtml;
            }
            else
            {
                throw 'unknwon type';
            }

        };

        this.getRowDataAsText = function (rowData) {
            var text = '{ ';
            for (var columnIndex = 0; columnIndex < rowData.length; columnIndex++) {
                if (rowData[columnIndex]) {
                    text += this.colHeaders[columnIndex] + '="' + rowData[columnIndex] + '"; ';
                }
            }
            text += '}';

            return text;
        };
    };

    handsontableScript(elements[0]);
    handsontableScript(elements[1]);
    handsontableScript(elements[2]);

});

function handsontableScript(element, handsontableObj)
{
    // DOM elements:
    var
      handsontableDiv = document.getElementById("handsontable_" + element.typeId),
      searchField = document.getElementById("search_field_" + element.typeId),
      searchResult = document.getElementById("search_result_" + element.typeId);
    
    // Other global variables:
    var searchResultCount = 0;
  
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
                    
                    var rowHtml = element.getRowAsHtmlResultItem(rowIndex, rowData);
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

}