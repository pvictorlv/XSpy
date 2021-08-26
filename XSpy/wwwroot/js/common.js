require.config({
    baseUrl: "/libs",
    urlArgs: "nV=10000",
    paths: {
        "jquery": "jquery/jquery.min",
        "bootstrap": "bootstrap/dist/js/bootstrap.bundle.min",
        "toastr": "toastr/toastr.min",
        "jquery-validate": "jquery-validation/dist/jquery.validate.min",
        "pace": "pace/pace.min",
        "jqueryui": "jqueryui/jquery-ui.min",
        "moment": "moment.js/moment-with-locales.min",
        "scrollbars": "overlayscrollbars/js/jquery.overlayScrollbars.min",
        "pdfmake": "pdfmake/pdfmake.min",
        "gallery": "ekko-lightbox/ekko-lightbox.min",
        "vfs_fonts": "pdfmake/vfs_fonts.min",
        "jszip": "https://cdnjs.cloudflare.com/ajax/libs/jszip/2.5.0/jszip.min",
        "datatables": "https://cdn.datatables.net/1.10.23/js/jquery.dataTables.min",
        'datatables.net-bs4': 'https://cdn.datatables.net/1.10.23/js/dataTables.bootstrap4.min',
        'responsive.bootstrap': 'https://cdn.datatables.net/responsive/2.2.7/js/responsive.bootstrap4.min',
        'datatables.net-responsive': 'https://cdn.datatables.net/responsive/2.2.7/js/dataTables.responsive.min',
        'datatables.net-buttons': 'https://cdn.datatables.net/buttons/1.6.5/js/dataTables.buttons.min',
        'buttons.bootstrap4': 'https://cdn.datatables.net/buttons/1.6.5/js/buttons.bootstrap4.min',
        'buttons.html5': 'https://cdn.datatables.net/buttons/1.6.5/js/buttons.html5.min',
        'buttons.print': 'https://cdn.datatables.net/buttons/1.6.5/js/buttons.print.min',
        "sweetalert": "sweetalert2/dist/sweetalert2.min",
        "select2": "select2/js/select2.full.min",
        "ichecks": "iCheck/icheck.min",
        "tables": "../js/tables/table-defaults",
        "main": "../js/main",
        "adminlte": "../js/adminlte",
        "utils": "../js/utils",
        "jsrender": "jsrender/jsrender.min",
        'card': "card/jquery.card.min",
        "userModal": "../js/app/users/modal",

    },
    waitSeconds: 0,
    shim: {
        "ichecks": {
            "deps": ["jquery"],
            "bootstrap": "bootstrap/dist/js/bootstrap.bundle"
        },
        "jqueryui": {
            "deps": ["jquery"]
        },
        "jsrender": {
            "deps": ["jquery"]
        },
        "gallery": {
            "deps": ["jquery"]
        },
        "main": {
            "deps": ["bootstrap", "adminlte", "scrollbars"]
        },
        "scrollbars": {
            "deps": ["bootstrap"]
        },
        "adminlte": {
            "deps": ["bootstrap", 'jquery', 'jqueryui', "scrollbars"]
        },
        "bootstrap": {
            "deps": ["jquery", "jqueryui"]
        },
        "vfs_fonts": {
            "deps": ["pdfmake"]
        },
        "datatables": {
            "deps": ["jquery", "bootstrap", "vfs_fonts", "jszip"]
        },
        "card": {
            "deps": ["jquery", "bootstrap"]
        },
        "datatables.net-responsive": {
            "deps": [
                "datatables", "datatables.net-bs4", "datatables.net-buttons", "buttons.bootstrap4", "buttons.print",
                "buttons.html5", "vfs_fonts"
            ]
        },
        "tables": {
            "deps": ["responsive.bootstrap"]
        },
        "responsive.bootstrap": {
            "deps": ["datatables.net-responsive"]
        }
    },
    map: {
        '*': {
            'datatables.net': 'datatables'
        }
    }
});

requirejs(["adminlte"]);