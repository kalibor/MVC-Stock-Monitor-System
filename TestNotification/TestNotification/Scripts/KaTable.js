(function ($) {
    $.fn.extend({
        SetTable: function (obj) {
            var table = $(this);
            if (table.hasClass('KaTable')) {

                var option = table.data('option');
                if (obj) {
                    option = $.extend(true, {
                        sourcelink: '',
                        searchoption: {},
                        columnslist: [{ field: '', title: '', template: function (data) { } }]
                    }, obj);
                    table.data('option', option);
                }
            }
        },
        Read(obj, nowpage,func) {
            var table = $(this);
            if (table.hasClass('KaTable')) {

                var option = table.data('option');
                if (option) {
                    option.searchoption = $.extend(true, option.searchoption, obj);
                    table.data('option', option);
                }

                ReSetPage(table, nowpage);
                AjaxLoadSource(table, func);
           
            }
        },
        Sort: function (columnname, desc) {
            var table = $(this);
            if (!columnname){
                AjaxLoadSource(table);
            }
            else {
                var source = table.data('source');
               
                if ($.isArray(source)) {
                    var data = source.sort(function (a, b) {
                        if (desc) {
                            return b[columnname] - a[columnname];
                        } else {
                            return a[columnname] - b[columnname];
                        }
                            
                    });
                   table.data('source', data);
                   ExportData(table);
                }
            }
        },
        SetPageSize: function (pagesize) {
            var table = $(this);
            if (table.hasClass('KaTable')) {
                if (pagesize) {
                    var tableinfo = table.data('pageinfo');
                    if (!tableinfo) {
                        tableinfo = {};
                    }
                    tableinfo.pagesize = pagesize;

                    table.data('pageinfo', tableinfo)
                }
            }
        },

    });


    function AjaxLoadSource(table,func) {
        var option = table.data('option');
        if (!option) {
            return false;
        }
            $.ajax({
                url: option.sourcelink,
                type: 'post',
                data: JSON.stringify(option.searchoption),
                contentType: 'application/json',
                dataType: 'json',
                success: function (data) {
                    if (Array.isArray(data)) {
                        table.data('source', data);
                        ExportData(table);
                        if ($.isFunction(func)) {
                            func();
                        }
                    }
                }
            });
    
   

    }

    function FillTh(table) {
        var option = table.data('option');
        if (!option) {
            return false;
        }

        var columnslist = option.columnslist;
        if (columnslist.length) {
            var thead = $('<thead></thead>');
            var tr = $('<tr>');

            $.each(columnslist, function (index, value) {
                var th = $('<th>');
                th.attr('id', 'Ka-' + value.field)
                th.data('field', value.field);
                if (value.template) {

                    th.data('template', value.template)
                }

                if (value.sort) {
                    th.on('click', function () {
                        table.Sort(value.field);
                    })
                }

                th.html(value.title)
                th.appendTo(tr)
            });
            tr.appendTo(thead);
            thead.appendTo(table);
        }

    };




    function FillTd(table, pagedsource) {
        var ths = table.find('th');
   
        tbody = $('<tbody>');
        $.each(pagedsource, function (index, value) {

            var tr = $('<tr>');
            ths.each(function () {
                var th = $(this);
                var target = value[th.data('field')];
                if (target) {
                    var td = $('<td>').addClass(th.data('field'));
                    var template = th.data('template');
                    if (template && typeof template == 'function') {
                        var inner = template(target)
                        $(inner).appendTo(td);
                    }
                    else {
                        td.html(target);
                    }
                    td.appendTo(tr);
                }

            });

            tr.appendTo(tbody);
        });

        tbody.appendTo(table);
    }

    function FillFooter(table) {
        var pageinfo = table.data('pageinfo');

        if (pageinfo) {
            var tfoot = $('<tfoot>');
            var tr = $('<tr>');
            var td = $('<td colspan="10000">');
            var ul = $('<ul class="pagedlist">');

            ul.on('click', 'li:not(.active)', function () {
                var page = $(this).data('nowpage');
                if (pageinfo.nowpage !== page) {
                    pageinfo.nowpage = page;
                    ReSetPage(table, page);
                    ExportData(table);
                }

            });

            for (var i = 1; i <= pageinfo.maxpage; i++) {

                var li = $('<li class="pageditem">');
                li.html(i).data('nowpage', i);

                if (pageinfo.nowpage == i) {
                    li.addClass('active ')
                }

                li.appendTo(ul);
            }
            ul.appendTo(td)
            td.appendTo(tr);
            tr.appendTo(tfoot);
            tfoot.appendTo(table);
        }
    }

    function ReSetPage(table, nowpage) {
        if (nowpage && nowpage >=1) {
            var pageinfo = table.data('pageinfo');
            if (!pageinfo) {
                pageinfo = {};
            }
         
            pageinfo.nowpage = nowpage;
            table.data('pageinfo', pageinfo);
        }
    }

    function ExportData(table) {
        var obj = table.data('pageinfo');
        var pageinfo = {};
        pageinfo = $.extend(true, {
            nowpage: 1,
            pagesize: 10,
            maxpage: 0,

        }, obj);
     
        var source = [];


        var SouceCount = table.data('source').length;

        pageinfo.maxpage = Math.floor(SouceCount / pageinfo.pagesize);

        if (SouceCount % pageinfo.pagesize !=0) {
            pageinfo.maxpage ++ ;
        }
        

        if (pageinfo.nowpage > pageinfo.maxpage) {
            pageinfo.nowpage = pageinfo.maxpage;
        }

        var nowpage = pageinfo.nowpage;
        var pagesize = pageinfo.pagesize;

        table.data('pageinfo', pageinfo);
     
        var start = (nowpage - 1) * pagesize;
        var end = (nowpage - 1) * pagesize + pagesize
        var _PagedSource = table.data('source').slice(start, end)

        if (!table.find('thead').length) {
            FillTh(table);
        }
        table.find('tbody, tfoot').remove();

        FillTd(table, _PagedSource);
        FillFooter(table);
    }



})(jQuery);
