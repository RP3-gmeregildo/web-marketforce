function exportToExcel(html, fileName)
{
    var a = document.createElement('a');

    var html = $('#dvData').html();

    while (html.indexOf('á') != -1) html = html.replace('á', '&aacute;');
    while (html.indexOf('é') != -1) html = html.replace('é', '&eacute;');
    while (html.indexOf('í') != -1) html = html.replace('í', '&iacute;');
    while (html.indexOf('ó') != -1) html = html.replace('ó', '&oacute;');
    while (html.indexOf('ú') != -1) html = html.replace('ú', '&uacute;');
    while (html.indexOf('º') != -1) html = html.replace('º', '&ordm;');

    a.href = 'data:application/vnd.ms-excel,' + encodeURIComponent(html)
    a.download = fileName;
    a.click();
}

function exportToExcelDetalle(html, fileName) {
    var a = document.createElement('a');

    var html = $('#dvDataDetalle').html();

    while (html.indexOf('á') != -1) html = html.replace('á', '&aacute;');
    while (html.indexOf('é') != -1) html = html.replace('é', '&eacute;');
    while (html.indexOf('í') != -1) html = html.replace('í', '&iacute;');
    while (html.indexOf('ó') != -1) html = html.replace('ó', '&oacute;');
    while (html.indexOf('ú') != -1) html = html.replace('ú', '&uacute;');
    while (html.indexOf('º') != -1) html = html.replace('º', '&ordm;');

    a.href = 'data:application/vnd.ms-excel,' + encodeURIComponent(html)
    a.download = fileName;
    a.click();
}

function exportTableToCSV($table, filename) {

    var $rows = $table.find('tr:has(td,th)'),

        // Temporary delimiter characters unlikely to be typed by keyboard
        // This is to avoid accidentally splitting the actual contents
        tmpColDelim = String.fromCharCode(11), // vertical tab character
        tmpRowDelim = String.fromCharCode(0), // null character

        // actual delimiter characters for CSV format
        colDelim = '";"',
        rowDelim = '"\r\n"',

        // Grab text from table into CSV formatted string
        csv = '"' + $rows.map(function (i, row) {
            var $row = $(row),
                $cols = $row.find('td');

            if ($cols.length == 0)
                $cols = $row.find('th');

            return $cols.map(function (j, col) {
                var $col = $(col),
                    text = $col.text();

                text = text.trim();

                return text.replace(/"/g, '""'); // escape double quotes

            }).get().join(tmpColDelim);

        }).get().join(tmpRowDelim)
            .split(tmpRowDelim).join(rowDelim)
            .split(tmpColDelim).join(colDelim) + '"',

        // Data URI
        csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

    $(this)
        .attr({
            'download': filename,
            'href': csvData,
            'target': '_blank'
        });
}