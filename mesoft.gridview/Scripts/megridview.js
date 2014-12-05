/*
    megridview v0.1
    Developed By Mesut Talebi (mesut.talebi@yahoo.com)
    Open Source And no licence :) free to use 
*/

$(function () {

    // Get First Page
    LoadFirstPage();

    //Btn gridview-next click event
    $(document).on('click', '.gridview button.gridview-next', function () {
        var currentPage = $('#gridview-data-details span.CurrentPage').html();
        var pageSize = $('#gridview-data-details span.PageSize').html();
        //var lastPage = $('input[type=hidden]#TotalRecords').val();

        if ($('.gridview button.gridview-next').is(':enabled')) {
            NextPage(currentPage, pageSize);
        }
    });

    //Btn gridview-prev click event
    $(document).on('click', '.gridview button.gridview-prev', function () {
        var currentPage = $('#gridview-data-details span.CurrentPage').html();
        var pageSize = $('#gridview-data-details span.PageSize').html();
        //var lastPage = $('input[type=hidden]#TotalRecords').val();

        if ($('.gridview button.gridview-prev').is(':enabled')) {
            PreviousPage(currentPage, pageSize);
        }
    });

    //Change PageSize
    $(document).on('click', '.gridview div.gridview-itemization ul.dropdown-menu li a', function () {
        var $li = $(this).parent();

        var size = $($li).data('value');
        var pageSize = $('input[type=hidden]#PageSize').val();
        if (size != pageSize)
            ChangePageSize(size);

        event.preventDefault();
    });


    //Pressing Enter event for page number input
    $(document).on('keypress', '.gridview input[type=number].gridview-secondaryPaging', function (e) {
        var key = e.which;
        if (key == 13) {
            var pageSize = $('input[type=hidden]#PageSize').val();
            var totalRecords = $('input[type=hidden]#TotalRecords').val();
            var lastPage = Math.ceil(totalRecords / pageSize);

            var page = parseInt($(this).val());

            if (page < 1)
                page = 1;
            if (page > lastPage)
                page = lastPage;

            GotoPage(page, pageSize);
        }
    });
});

function LoadFirstPage() {
    var $defaultPageSize = $('#viewport').data('default-pagesize');

    LoadData(1, $defaultPageSize);
}

function LoadData(page, pageSize) {
    //Start Loading
    ShowLoader();
    var $getDataUrl = $('#viewport').data('getdata-function');

    $.ajax({
        url: $getDataUrl,
        data: { page: page, pageSize: pageSize },
        success: function (data) {
            UpdateGridView(data);
        },
        complete: function () {
            HideLoader();
        }
    });
}

function ShowLoader() {
    //$('#viewport').css("background-color", "rgba(200,200,200, 0.4)");
    $('.gridview.gridview-loader').show();
}

function HideLoader() {
    $('.gridview.gridview-loader').hide();
    $('#gridview table.gridview-list-items').show();
}

function UpdateGridView(data) {
    //Set Table Inner
    $('#viewport .gridview-canvas').html(data);

    var totalRecords = $('#gridview-data-details span.TotalRecords').html();
    var currentPage = $('#gridview-data-details span.CurrentPage').html();
    var pageSize = $('#gridview-data-details span.PageSize').html();


    //Set Showing records    
    var showingStart = (currentPage - 1) * pageSize + 1;
    var showingEnd = currentPage * pageSize;
    if (showingEnd > totalRecords)
        showingEnd = totalRecords;

    $('.gridview span.gridview-start').html(showingStart);
    $('.gridview span.gridview-end').html(showingEnd);

    //Set Total Items Available
    $('.gridview span.gridview-count').html(totalRecords);

    //Set Current Page
    $('.gridview input[type=number].gridview-secondaryPaging').val(currentPage);

    //Set PageSize
    $('.gridview span.selected-label').html(pageSize);

    //Set Btn Gridview pages count (span.gridview-pages)


    var lastPage = Math.ceil(totalRecords / pageSize);

    //Update Total Pages 
    $('.gridview div.gridview-pagination span.gridview-pages').html(lastPage);

    //Set Btn gridview-next and btn gridview-prev enabled
    if (lastPage <= 1) {
        $('.gridview button.gridview-next').attr('disabled', 'disabled');
        $('.gridview button.gridview-prev').attr('disabled', 'disabled');
    }
    else if (currentPage > 1) {
        $('.gridview button.gridview-next').removeAttr('disabled');
        $('.gridview button.gridview-prev').removeAttr('disabled');
    }
    else if (currentPage == 1) {
        $('.gridview button.gridview-next').removeAttr('disabled');
        $('.gridview button.gridview-prev').attr('disabled', 'disabled');
    }
    else if (currentPage == lastPage) {
        $('.gridview button.gridview-next').attr('disabled', 'disabled');
        $('.gridview button.gridview-prev').removeAttr('disabled');
    }
}

function NextPage(currentPage, pageSize) {
    //Check For Last Page
    var page = parseInt(currentPage) + 1;
    LoadData(page, pageSize);
}

function PreviousPage(currentPage, pageSize) {
    //Check For First Page
    var page = parseInt(currentPage) + 1;
    LoadData(currentPage - 1, pageSize);
}

function GotoPage(page, pageSize) {
    //Check For Correct Page Number
    LoadData(page, pageSize);
}

function ChangePageSize(pageSize) {
    LoadData(1, pageSize);
}