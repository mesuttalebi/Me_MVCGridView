/*
    megridview v0.3.1
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
                if ($('button.gridview-next', gridview).is(':enabled')) {
                    NextPage(gridview);
                }
            });

            //Btn gridview-prev click event
            $(gridview).on('click', 'button.gridview-prev', function () {
                if ($('button.gridview-prev', gridview).is(':enabled')) {
                    PreviousPage(gridview);
                }
            });

            //Change PageSize
            $(gridview).on('click', 'div.gridview-itemization ul.dropdown-menu li a', function (ev) {
                ev.preventDefault();

                var $li = $(this).parent();

                var size = $($li).data('value');
                var pageSize = $gridviewObject.ItemsPerPage;
                if (size != pageSize)
                    ChangePageSize(size, gridview);               
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

                    GotoPage(page, gridview);
                }
            });

            //Searching data
            $(gridview).on('click', '.search button', function() {
                var $searchTerm = $('.search input[type=search]', gridview).val();

                if ($searchTerm.length >= 0) {
                    SearchData($searchTerm, gridview);
                }
            });

            //Pressing Enter event for Search input
            $(gridview).on('keypress', '.search input[type=search]', function (e) {
                var key = e.which;
                if (key == 13) {
                    var $searchTerm = $('.search input[type=search]', gridview).val();

                    if ($searchTerm.length >= 0) {
                        SearchData($searchTerm, gridview);
                    }
                }
            });

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

            //Filtering Data
            $(gridview).on('click', '.filter', function (ev) {
                ev.preventDefault();

                FilterData(this, gridview);

                var p = $(this).parents('.filter-parent');
                var obj = $('.active', p)[0].nodeName.toLowerCase();

                $(obj + '.active', p).removeClass('active');
                $(this).parents(obj).addClass('active');                
            });
        });

        function FilterData(object, gridview) {
            //Collecting new filter values
            var $column = $(object).data('filter-column');
            var $value = $(object).data('filter-value');
            var $operator = $(object).data('filter-operator');
            var $conjunction = $(object).data('filter-conjunction');

            //creating new filter object
            var filterObj = { Column: $column, Value: $value, Operator: $operator, Conjunction: $conjunction };

            //Getting existing filters
            var filters = $gridviewObject.Filters;

            if (filters !== undefined && filters !== null) {
                //Searching for same column filters                   
                var foundFlag = false;
                for (var i = 0; i < filters.length; i++) {
                    if (filterObj.Column === filters[i].Column) {
                        filters[i] = filterObj;
                        foundFlag = true;
                        break;
                    }
                }

                //same column not found, 
                //add new column filter to filters
                if (!foundFlag) {
                    $gridviewObject.Filters.push(filterObj);
                }

                LoadData(gridview);

            } else {
                $gridviewObject.Filters = [];
                $gridviewObject.Filters.push(filterObj);
                LoadData(gridview);
            }
        }
       
        function Init(obj) {
            //add icon place holder for sortable fields
            $('th.sortable', obj).append('<span class="pull-right fa"></span>');

            var gridObject = $('.gridview-data-details', obj).html();
            $gridviewObject = JSON.parse(gridObject);

            //console.log($gridviewObject);
        }

        function LoadFirstPage(obj) {
            var $defaultPageSize = $('.gridview-viewport', obj).data('default-pagesize');

            var data = { "CurrentPage": 1, "ItemsPerPage": $defaultPageSize };

            GetData(data, obj);
        }

        function LoadData(obj) {
                      
            var data = {
                "CurrentPage": $gridviewObject.CurrentPage,
                "ItemsPerPage": $gridviewObject.ItemsPerPage,
                "SearchTerm": $gridviewObject.SearchTerm,
                "Sort": $gridviewObject.Sort,
                "Filters": $gridviewObject.Filters
            };
            
            GetData(data, obj);
        }

        function GetData(data, obj) {
            //Start Loading
            ShowLoader(obj);

            var $getDataUrl = $('.gridview-viewport', obj).data('getdata-function');

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

        function NextPage(obj) {
            //Check For Last Page
            var page = parseInt($gridviewObject.CurrentPage) + 1;

            $gridviewObject.CurrentPage = page;

            LoadData(obj);
        }

        function PreviousPage(obj) {
            //Check For First Page
            var page = parseInt($gridviewObject.CurrentPage) - 1;
            $gridviewObject.CurrentPage = page;
            LoadData(obj);
        }

        function GotoPage(page, obj) {
            //Check For Correct Page Number
            $gridviewObject.CurrentPage = page;

            LoadData(obj);
        }

        function ChangePageSize(pageSize, obj) {

            $gridviewObject.CurrentPage = 1;
            $gridviewObject.ItemsPerPage = pageSize;

            LoadData(obj);
        }

        function SortData(column, direction, obj) {
            var SortObject = { "SortColumn": column, "Direction": direction };

            //var $pageSize = $gridviewObject.ItemsPerPage; 
            //var $searchTerm = $gridviewObject.searchTerm; 
            $gridviewObject.Sort = SortObject;

            LoadData(obj);
        }

        function SearchData(searchTerm, obj) {
            $gridviewObject.SearchTerm = searchTerm;
            $gridviewObject.CurrentPage = 1;
            $gridviewObject.ItemsPerPage = $gridviewObject.PageOptions[0];
            LoadData(obj);
        }
    };    
}(jQuery));
