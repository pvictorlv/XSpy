require(['./common'],
    function(common) {
        require(['main'],
            function(main) {
                console.log('123');
            }
        );
    }
);