window.onload = function (){
    textModify();
    textModify2();
}
function textModify()
{
    var obj = document.getElementById("p");
    alert(obj.innerHTML);
    obj.innerHTML= "google coding";
}
function textModify2()
{
    var obj = document.getElementById("p2");
    alert(obj.innerText);
    obj.innerText= "knowledge";
}