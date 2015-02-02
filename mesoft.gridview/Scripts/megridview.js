/*
    megridview v0.2
    Developed By Mesut Talebi (mesut.talebi@yahoo.com)
    Open Source And no licence :) free to use 
*/
(function ($) {   
    $.fn.meGridView = function () {

        var $gridviewObject = new Object();

        return this.each(function () {

            //The gridview object that we are working on it
            var gridview = this;
           
            // Get First Page
            LoadFirstPage(gridview);

            //Btn gridview-next click event
            $(gridview).on('click', 'button.gridview-next', function () {
                var currentPage = $gridviewObject.CurrentPage;
                var pageSize = $gridviewObject.ItemsPerPage;

                if ($('button.gridview-next', gridview).is(':enabled')) {
                    NextPage(currentPage, pageSize, gridview);
                }
            });

            //Btn gridview-prev click event
            $(gridview).on('click', 'button.gridview-prev', function () {
                var currentPage = $gridviewObject.CurrentPage;
                var pageSize = $gridviewObject.ItemsPerPage;

                if ($('button.gridview-prev', gridview).is(':enabled')) {
                    PreviousPage(currentPage, pageSize, gridview);
                }
            });

            //Change PageSize
            $(gridview).on('click', 'div.gridview-itemization ul.dropdown-menu li a', function () {
                var $li = $(this).parent();

                var size = $($li).data('value');
                var pageSize = $gridviewObject.ItemsPerPage;
                if (size != pageSize)
                    ChangePageSize(size, gridview);

                event.preventDefault();
            });

            //Pressing Enter event for page number input
            $(gridview).on('keypress', 'input[type=number].gridview-secondaryPaging', function (e) {
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

                    GotoPage(page, pageSize, gridview);
                }
            });

            //Searching data
            $(gridview).on('click', '.search button', function () {
                var $searchTerm = $('.search input[type=search]', gridview).val();

                if ($searchTerm.length >= 0) {
                    LoadData(1, 10,gridview, $searchTerm);
                }
            })

            //Sorting Data
            $(gridview).on('click', 'th.sortable', function () {

                var $sortSpan = $(this).find('span');
                var $sortObject = $gridviewObject.Sort;

                if ($sortObject != null && $sortObject.Direction == 1) {
                    $sortSpan.removeClass('fa-chevron-up').addClass('fa-chevron-down');

                    //Sort Descending            
                    SortData($(this).data('sort'), 'Descending', gridview);
                }
                else {
                    $sortSpan.removeClass('fa-chevron-down').addClass('fa-chevron-up');
                    //sort Ascending
                    SortData($(this).data('sort'), 'Ascending', gridview);
                }
            });            
        });



        function Init(obj) {
            //add icon place holder for sortable fields
            $('th.sortable', obj).append('<span class="pull-right fa"></span>');

            var gridObject = $('.gridview-data-details', obj).html();
            $gridviewObject = JSON.parse(gridObject);
        }

        function LoadFirstPage(obj) {
            var $defaultPageSize = $('.gridview-viewport', obj).data('default-pagesize');

            LoadData(1, $defaultPageSize, obj);
        }

        function LoadData(page, pageSize, obj, searchTerm, sortObject) {
            //Start Loading
            ShowLoader(obj);

            var $getDataUrl = $('.gridview-viewport', obj).data('getdata-function');

            //retrieving search term
            if (searchTerm == undefined) {
                searchTerm = $('.search input[type=search]', obj).val();
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
                    UpdateGridView(data, obj);
                },
                complete: function () {
                    HideLoader(obj);
                }
            });
        }

        function ShowLoader(obj) {
            $('.gridview-canvas table', obj).hide();
            $('.gridview-loader', obj).show();
        }

        function HideLoader(obj) {
            $('.gridview-loader', obj).hide();
            $('.gridview-canvas table', obj).show();
        }

        function UpdateGridView(data, obj) {
            //Set Table Inner
            $('#viewport .gridview-canvas', obj).html(data);
            //Initialize gridview
            Init(obj);

            var totalRecords = $gridviewObject.TotalItems;
            var currentPage = $gridviewObject.CurrentPage;
            var pageSize = $gridviewObject.ItemsPerPage;

            //Set Showing records    
            var showingStart = (currentPage - 1) * pageSize + 1;
            var showingEnd = currentPage * pageSize;
            if (showingEnd > totalRecords)
                showingEnd = totalRecords;

            $('span.gridview-start', obj).html(showingStart);
            $('span.gridview-end', obj).html(showingEnd);

            //Set Total Items Available
            $('span.gridview-count', obj).html(totalRecords);

            //Set Current Page
            $('input[type=number].gridview-secondaryPaging', obj).val(currentPage);

            //Set PageSize
            $('span.selected-label', obj).html(pageSize);

            //Set Btn Gridview pages count (span.gridview-pages)


            var lastPage = Math.ceil(totalRecords / pageSize);

            //Update Total Pages 
            $('div.gridview-pagination span.gridview-pages', obj).html(lastPage);

            //Set Btn gridview-next and btn gridview-prev enabled
            if (lastPage <= 1) {
                $('button.gridview-next', obj).attr('disabled', 'disabled');
                $('button.gridview-prev', obj).attr('disabled', 'disabled');
            }
            else if (currentPage == lastPage) {
                $('button.gridview-next', obj).attr('disabled', 'disabled');
                $('button.gridview-prev', obj).removeAttr('disabled');
            }
            else if (currentPage > 1) {
                $('button.gridview-next', obj).removeAttr('disabled');
                $('button.gridview-prev', obj).removeAttr('disabled');
            }
            else if (currentPage == 1) {
                $('button.gridview-next', obj).removeAttr('disabled');
                $('button.gridview-prev', obj).attr('disabled', 'disabled');
            }


            //Update sorted columns
            if ($gridviewObject.Sort != null) {
                //clear all previous sorting
                $('th.sortable', obj).removeClass('sorted');
                $('th.sortable > span', obj).removeClass('fa-chevron-up').removeClass('fa-chevron-down');

                //find sorted column
                var sortableTh = $('th.sortable[data-sort=' + $gridviewObject.Sort.SortColumn + ']', obj);
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

        function NextPage(currentPage, pageSize, obj) {
            //Check For Last Page
            var page = parseInt(currentPage) + 1;
            LoadData(page, pageSize, obj);
        }

        function PreviousPage(currentPage, pageSize, obj) {
            //Check For First Page
            var page = parseInt(currentPage) + 1;
            LoadData(currentPage - 1, pageSize, obj);
        }

        function GotoPage(page, pageSize, obj) {
            //Check For Correct Page Number
            LoadData(page, pageSize, obj);
        }

        function ChangePageSize(pageSize, obj) {
            LoadData(1, pageSize, obj);
        }

        function SortData(column, direction, obj) {
            var SortObject = { "SortColumn": column, "Direction": direction };

            var $pageSize = $gridviewObject.ItemsPerPage; // $('#gridview-data-details span.PageSize').html();
            var $searchTerm = $gridviewObject.searchTerm; // $('.gridview .search input[type=search]').val();

            LoadData(1, $pageSize, obj, $searchTerm, SortObject);
        }
    };    
}(jQuery));
