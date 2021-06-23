require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'scrollbars', 'moment'],
            function($, tables, utils, scrollbars, moment) {
                $(document).ready(function() {

                    $(".contact-data").click(function() {
                        var $this = $(this);
                        var contactId = $this.data('id');
                        var contactName = $this.data('name');
                        $("#contactName").html("Mensagens de " + contactName);

                        $.getJSON("/api/appContact/" + contactId,
                            function (data) {
                                $("#msgList").html('');
                                for (let msg of data) {
                                    var date = moment(msg.messageDate).format("DD/MM/YY HH:mm");
                                    if (!msg.isOwn) {
                                        $("#msgList")
                                            .append(
                                                `<div class='direct-chat-msg'><div class="direct-chat-text"> ${msg.body
                                                }<small><span class="direct-chat-timestamp float-right">${date
                                                }</span></small></div></div></div>`);
                                    } else {
                                        $("#msgList")
                                            .append(
                                                `<div class='direct-chat-msg right'><div class="direct-chat-text right"> ${msg.body
                                                }<small><span class="direct-chat-timestamp float-right text-white">${date
                                                }</span></small></div></div></div>`);                                    }
                                }
                            });
                    });

                    $('#chat-box').overlayScrollbars({
                        height: '250px'
                    });

                    $('.direct-chat-messages').overlayScrollbars({
                        height: '250px'
                    });

                });
            }
        );
    }
);