document.addEventListener("DOMContentLoaded", function () {

    var personsKey = 'persons';
    var marriagesKey = 'marriages';
    var sonsKey = 'sons';

    elements.push(new element(personsKey, colHeaders_1, data_1));
    elements.push(new element(marriagesKey, colHeaders_2, data_2));
    elements.push(new element(sonsKey, colHeaders_3, data_3)); 

    var saveResult = document.getElementById("save_result");

    function element(typeId, colHeaders, data) {

        this.typeId = typeId;

        this.pendingChanges = {};
        this.pendingChangeCounter = 0;

        this.pendingDeletes = {};
        this.pendingDeleteCounter = 0;

        this.colHeaders = colHeaders;
        this.data = data;

        this.getRowAsHtmlResultItem = function (rowIndex, rowData) {

            var text = this.getRowDataAsText(rowData);
            var elementId = rowData[1];

            var onclick1 = "'/data/persondownwardtree?personId=" + elementId + "'";
            var link1 = '<a href="#" onclick="return popitup(' + onclick1 + ')">downward tree</a>';

            var onclick2 = "'/data/upwardtree?personId=" + elementId + "'";
            var link2 = '<a href="#" onclick="return popitup(' + onclick2 + ')">upwward tree</a>';

            var onclick3 = "'/data/personById?personId=" + elementId + "'";
            var link3 = '<a href="#" onclick="return popitup(' + onclick3 + ')">edit person</a>';

            var onclick4 = "'/data/marriageByPersonId?personId=" + elementId + "'";
            var link4 = '<a href="#" onclick="return popitup(' + onclick4 + ')">edit marriage</a>';

            /* TODO: implement delete
            HTMLElement.prototype.doHello = function(thing){
                alert('Hello World from ' + thing);
            }; */

            var linkHtml = '<div>' + link1 + ' | ' + link2 + ' | ' + link3 + ' | ' + link4 + ' | <a href="#" onclick="addToDeleteList(\'' + rowIndex + '\',\'' + this.typeId + '\')">delete</a> | ' + text + '</div>';
            return linkHtml;

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

        this.addPendingChange = function (rowIndex, rowData) {
            if (!(rowIndex in this.pendingChanges)) {
                this.pendingChangeCounter++;
            }
            this.pendingChanges[rowIndex] = rowData;
            visualizePendingChanges();
        };

        this.addPendingDelete = function (rowIndex, rowData) {
            if (!(rowIndex in this.pendingDeletes)) {
                this.pendingDeleteCounter++;
            }
            this.pendingDeletes[rowIndex] = rowData;
            visualizePendingChanges();
        };
    };

    /**
    * Shows the pending changes as text.
    */
    function visualizePendingChanges() {
        var pendingChangesDiv = document.getElementById('pending_changes');

        var text = "Pending changes:\r\n";
        for (var elementIndex = 0; elementIndex < elements.length; elementIndex++) {
            var element = elements[elementIndex];
            text += '\r\n' + element.typeId + ' (' + element.pendingChangeCounter + ')' + ":\r\n";
            for (var rowIndex in element.pendingChanges) {
                var rowData = element.pendingChanges[rowIndex];
                text += element.getRowDataAsText(rowData) + "\r\n";
            }
        }

        var text = "Pending deletes:\r\n";
        for (var elementIndex = 0; elementIndex < elements.length; elementIndex++) {
            var element = elements[elementIndex];
            text += '\r\n' + element.typeId + ' (' + element.pendingDeleteCounter + ')' + ":\r\n";
            for (var rowIndex in element.pendingDeletes) {
                var rowData = element.pendingDeletes[rowIndex];
                text += element.getRowDataAsText(rowData) + "\r\n";
            }
        }

        pendingChangesDiv.innerText = text;
    }

    /**
    * Click on Save button
    **/
    Handsontable.Dom.addEvent(save, 'click', function () {

        var data = getDataToSend();

        // save all cell's data
        $.ajax({
            url: "/Data/Save",
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json",
            complete: function (res) {
                if (res.resultText === '"ok"') {
                    saveResult.innerText = 'Data saved';
                }
                else {
                    saveResult.innerText = 'Save error';
                }
            }
        });
    });

    /**
    * Collects all data to send into a single object.
    **/
    function getDataToSend() {

        var data = {
            toInsertOrUpdate: {},
            toRemove: {}
        };

        for (var elementIndex = 0; elementIndex < elements.length; elementIndex++) {
            var element = elements[elementIndex];

            // pending 'insert or update':
            var pendingChanges = [];
            var changeCount = 0;
            for (var rowIndex in element.pendingChanges) {
                pendingChanges[changeCount] = element.pendingChanges[rowIndex];
                changeCount++;
            }
            data.toInsertOrUpdate[element.typeId] = pendingChanges;

            // pending 'delete':
            var pendingDeletes = [];
            var deleteCount = 0;
            for (var rowIndex in element.pendingDeletes) {
                pendingDeletes[deleteCount] = element.pendingDeletes[rowIndex];
                deleteCount++;
            }
            data.toRemove[element.typeId] = pendingDeletes;
        }

        return data;
    }

    handsontableScript(elements[0]);
    handsontableScript(elements[1]);
    handsontableScript(elements[2]);

});