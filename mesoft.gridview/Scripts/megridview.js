/*
    megridview v0.1
    Developed By Mesut Talebi (mesut.talebi@yahoo.com)
    Open Source And no licence :) free to use 
*/

var $gridviewObject = new Object();

$(function () {
    // Get First Page
    LoadFirstPage();

    //Btn gridview-next click event
    $(document).on('click', '.gridview button.gridview-next', function () {
        var currentPage = $gridviewObject.CurrentPage;
        var pageSize = $gridviewObject.ItemsPerPage;

        if ($('.gridview button.gridview-next').is(':enabled')) {
            NextPage(currentPage, pageSize);
        }
    });

    //Btn gridview-prev click event
    $(document).on('click', '.gridview button.gridview-prev', function () {
        var currentPage = $gridviewObject.CurrentPage;
        var pageSize = $gridviewObject.ItemsPerPage;

        if ($('.gridview button.gridview-prev').is(':enabled')) {
            PreviousPage(currentPage, pageSize);
        }
    });

    //Change PageSize
    $(document).on('click', '.gridview div.gridview-itemization ul.dropdown-menu li a', function () {
        var $li = $(this).parent();

        var size = $($li).data('value');
        var pageSize = $gridviewObject.ItemsPerPage;
        if (size != pageSize)
            ChangePageSize(size);

        event.preventDefault();
    });

    //Pressing Enter event for page number input
    $(document).on('keypress', '.gridview input[type=number].gridview-secondaryPaging', function (e) {
        var key = e.which;
        if (key == 13) {
            var pageSize = $gridviewObject.ItemsPerPage;
            var totalRecords = $gridviewObject.TotalItems;
            var lastPage = Math.ceil(totalRecords / pageSize);

            var page = parseInt($(this).val());

            if (page < 1)
                page = 1;
            if (page > lastPage)
                page = lastPage;

            GotoPage(page, pageSize);
        }
    });

    //Searching data
    $(document).on('click', '.gridview .search button', function () {
        var $searchTerm = $('.gridview .search input[type=search]').val();

        if ($searchTerm.length >= 0) {
            LoadData(1, 10, $searchTerm);
        }
    })

    //Sorting Data
    $(document).on('click', '.gridview th.sortable', function () {

        var $sortSpan = $(this).find('span');
        var $sortObject = $gridviewObject.Sort;

        if ($sortObject != null && $sortObject.Direction == 1) {
            $sortSpan.removeClass('fa-chevron-up').addClass('fa-chevron-down');

            //Sort Descending            
            SortData($(this).data('sort'), 'Descending');
        }
        else {
            $sortSpan.removeClass('fa-chevron-down').addClass('fa-chevron-up');
            //sort Ascending
            SortData($(this).data('sort'), 'Ascending');
        }
    });
});

function Init() {
    //add icon place holder for sortable fields
    $('.gridview th.sortable').append('<span class="pull-right fa"></span>');

    var gridObject = $('#gridview-data-details').val();
    $gridviewObject = JSON.parse(gridObject);
}

function LoadFirstPage() {
    var $defaultPageSize = $('#viewport').data('default-pagesize');

    LoadData(1, $defaultPageSize);
}

function LoadData(page, pageSize, searchTerm, sortObject) {
    //Start Loading
    ShowLoader();

    var $getDataUrl = $('#viewport').data('getdata-function');

    //retrieving search term
    if (searchTerm == undefined) {
        searchTerm = $('.gridview .search input[type=search]').val();
    }

    //retrieving sort object
    if (sortObject == undefined) {
        //try to get sort object from gridviewObject
        sortObject = $gridviewObject.Sort;
    }

    var data = { "CurrentPage": page, "ItemsPerPage": pageSize, "SearchTerm": searchTerm };

    if (sortObject != undefined) {
        data.Sort = sortObject;
    }

    $.ajax({
        url: $getDataUrl,
        type: 'post',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        success: function (data) {
            UpdateGridView(data);
        },
        complete: function () {
            HideLoader();
        }
    });
}

function ShowLoader() {
    $('.gridview .gridview-canvas table').hide();
    $('.gridview.gridview-loader').show();
}

function HideLoader() {
    $('.gridview.gridview-loader').hide();
    $('.gridview .gridview-canvas table').show();
}

function UpdateGridView(data) {
    //Set Table Inner
    $('#viewport .gridview-canvas').html(data);
    //Initialize gridview
    Init();

    var totalRecords = $gridviewObject.TotalItems;//$('#gridview-data-details span.TotalRecords').html();
    var currentPage = $gridviewObject.CurrentPage;//$('#gridview-data-details span.CurrentPage').html();
    var pageSize = $gridviewObject.ItemsPerPage;//$('#gridview-data-details span.PageSize').html();


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

    //Update sorted columns
    if ($gridviewObject.Sort != null) {
        //clear all previous sorting
        $('.gridview th.sortable').removeClass('sorted');
        $('.gridview th.sortable > span').removeClass('fa-chevron-up').removeClass('fa-chevron-down');

        //find sorted column
        var sortableTh = $('.gridview th.sortable[data-sort=' + $gridviewObject.Sort.SortColumn + ']');
        console.log(sortableTh);

        $(sortableTh).addClass('sorted');
        var $sortSpan = $(sortableTh).find('span');

        if ($gridviewObject.Sort.Direction == 1) {
            $sortSpan.addClass('fa-chevron-up');
        }
        else if ($gridviewObject.Sort.Direction == 2) {
            $sortSpan.addClass('fa-chevron-down');
        }
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

function SortData(column, direction) {
    var SortObject = { "SortColumn": column, "Direction": direction };

    var $pageSize = $gridviewObject.ItemsPerPage; // $('#gridview-data-details span.PageSize').html();
    var $searchTerm = $gridviewObject.searchTerm; // $('.gridview .search input[type=search]').val();

    LoadData(1, $pageSize, $searchTerm, SortObject);
}