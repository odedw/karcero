/**
 * Created by oded on 30/08/2014.
 */
$(function(){
    mixpanel.track('Page View - Karcero Demo');
    var u = undefined;
    setTimeout(function(){

        var config = {
            width: $('.wrapper').width(),
            height: $('.wrapper').width(),
            params: { enableDebugging:"1" }
        };
        console.log($('.wrapper').css('width'));
        u = new UnityObject2(config);

        var $missingScreen = jQuery("#unityPlayer").find(".missing");
        var $brokenScreen = jQuery("#unityPlayer").find(".broken");
        $missingScreen.hide();
        $brokenScreen.hide();

        u.observeProgress(function (progress) {
            switch(progress.pluginStatus) {
                case "broken":
                    $brokenScreen.find("a").click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        u.installPlugin();
                        return false;
                    });
                    $brokenScreen.show();
                    break;
                case "missing":
                    $missingScreen.find("a").click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        u.installPlugin();
                        return false;
                    });
                    $missingScreen.show();
                    break;
                case "installed":
                    $missingScreen.remove();
                    break;
                case "first":
                    break;
            }
        });
        u.initPlugin(jQuery("#unityPlayer")[0], "/karcero/content/Unity.unity3d");
    });

    var keys = ['size','randomness','sparseness','deadends','room-size','room-count'];
    function methodChanged(){
        var definitions = [];
        for(var i=0;i<keys.length;i++){
            definitions.push($('#'+keys[i]).val());
        }
        if (u){
            u.getUnity().SendMessage("ControllerObject", "GenerateWithParameters", JSON.stringify(definitions));
        }
    };
    $(".api-select").dropkick({
        change:methodChanged
    });

});


