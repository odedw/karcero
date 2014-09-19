/**
 * Created by oded on 30/08/2014.
 */
$(function(){
    function methodChanged(){
        var definitions = {
            'size': $('#size').val(),
            'randomness': $('#randomness').val(),
            'sparseness': $('#sparseness').val(),
            'deadends': $('#deadends').val(),
            'room-size': $('#room-size').val(),
            'room-count': $('#room-count').val()
        }
    };
    mixpanel.track('Page View - Karcero Demo');
    $(".api-select").dropkick({
        change:methodChanged
    });
});