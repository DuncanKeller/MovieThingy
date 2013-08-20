// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    // buttons
    var selectFolderButton;
    var sortTitleButton;
    var sortYearButton;
    var sortRatingButton;
    // text boxes
    var filterTitle;
    var filterActors;
    var filterDirector;
    // drop down
    var filterGenre;

    var addElement = function (type, idName) {
        var element = document.createElement(type);
        element.id = idName;
        
        return element;
    };

    var addTextBox = function (idName) {
        var element = addElement("input", idName);
        element.type = "text";
        return element;
    };

    var addDropDown = function (idName) {
        
    };

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                init();
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }


    };

    var init = function () {
        selectFolderButton =  addElement("button", "selectFolderButton");
        sortTitleButton = addElement("button", "sortTitleButton");
        sortYearButton = addElement("button", "sortYearButton");
        sortRatingButton = addElement("button", "sortRatingButton");
        filterTitle = addTextBox("filterTitle");
        filterActors = addTextBox("filterActors");
        filterDirector = addTextBox("filterDirector");

        
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
})();
