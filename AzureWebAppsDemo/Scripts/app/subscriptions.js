$(document).ready(function () {

    var queueColumns = [
     {
         searchable: true, orderable: true, name: 'First Name', data: 'FirstName'
     },
     {
         searchable: true, orderable: true, name: 'Last Name', data: 'LastName'
     },
     {
         searchable: true, orderable: true, name: 'Email', data: 'Email'
     },
     {
         searchable: true, orderable: true, name: 'Received Date', data: 'CreatedDate',
         render: function (data, type, row) {
             return data != null ? moment(data).format('DD/MM/YYYY HH:mm') : "";
         }
     }
    ];

    var subscriptionsColumns = [
       {
           searchable: true, orderable: true, name: 'First Name', data: 'FirstName'
       },
       {
           searchable: true, orderable: true, name: 'Last Name', data: 'LastName'
       },
       {
           searchable: true, orderable: true, name: 'Email', data: 'Email'
       },
       {
           searchable: true, orderable: true, name: 'Created Date', data: 'CreatedDate',
           render: function (data, type, row) {
               return data != null ? moment(data).format('DD/MM/YYYY HH:mm') : "";
           }
       }
    ];

    $('#queueTable').DataTable({
        serverSide: true,
        deferRender: true,
        paging: true,
        pageLength: 5,
        colReorder: false,
        columns: subscriptionsColumns,
        processing: false,
        ajax: {
            url: 'api/queue',
            type: 'POST'
        }
    });

    $('#subscriptionsTable').DataTable({
        serverSide: true,
        deferRender: true,
        paging: true,
        pageLength: 5,
        colReorder: false,
        columns: subscriptionsColumns,
        processing: false,
        ajax: {
            url: 'api/subscriptions',
            type: 'POST'
        }
    });

    setInterval(function () {
        $('#subscriptionsTable').DataTable().draw(false);
    }, 3000);

    setInterval(function () {
        $('#queueTable').DataTable().draw(false);
    }, 3000);

    $('#addSubscriptionDialog').dialog({
        modal: true,
        title: 'Add Subscription',
        autoOpen: false,
        closeOnEscape: false,
        width: '380px',
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        buttons: [
            {
                text: 'Submit',
                click: function () {
                    $('#addSubscriptionForm').submit();
                }
            },
            {
                text: 'Cancel',
                click: function () {
                    $(this).dialog('close');
                }
            }
        ]
    });

    $('#btnAddSubscription').on('click', function () {
        $('#addSubscriptionDialog').dialog('open');
    });
});