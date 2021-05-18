//var postNumber = 0;
//var userName = "projects_association";
////var currentPost = document.getElementsByTagName('article');
//var currentScroll = 0;
////var currentLikeButton = currentPost.getElementsByClassName('wpO6b  ')[1];
//const delay = ms => new Promise(res => setTimeout(res, ms));


//async function Like() {
//    if (document.getElementsByTagName('article')[postNumber].getElementsByTagName('a')[0].text == userName) {
//        document.getElementsByTagName('article')[postNumber].getElementsByClassName('wpO6b')[1].click();
//    }
//    postNumber++;
//    await delay(3000)
//}

//console.log("Like script started!")
//document.getElementsByTagName('article')[postNumber].getElementsByClassName('wpO6b')[1].scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });

//while (postNumber < 50) {
//    Like();
//    document.getElementsByTagName('article')[postNumber].getElementsByClassName('wpO6b')[1].scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
//}


var postNumber = 1;
const delay = ms => new Promise(res => setTimeout(res, ms));
var element = document.getElementsByTagName('article')[0];
var userName = "motionlovers";

async function Like() {
    if (element.getElementsByClassName('sqdOP yWX7d')[0].text === userName && element.getElementsByTagName('svg')[1].ariaLabel === "Нравится") {
        await delay(1000);
        console.log(userName + " IS LIKED!");
        element.getElementsByClassName('wpO6b')[1].click();
    }
    postNumber++;
    element = element.nextElementSibling;
}

console.log("Like script started!");

while (postNumber < 50) {

    element.getElementsByClassName('wpO6b')[1].scrollIntoView({ block: "center", inline: "nearest" });
    console.log("Post number: " + postNumber);
    Like();
    await delay(2000);
}

// С помощью этого метода спарсил логины подписок
function InstaSubscriptionsPrse() {
    var nameCount = 0;
    var subCount = parseFloat(document.querySelectorAll('span[class="g47SY "]')[2].innerText);
    var acc = document.getElementsByClassName('isgrP')[0].getElementsByTagName('li')[0]
    const delay = ms => new Promise(res => setTimeout(res, ms));
    console.log("Sub Count: " + subCount);

    while (nameCount < subCount) {
        acc.scrollIntoView();
        console.log('"' + acc.getElementsByClassName('FPmhX notranslate  _0imsa ')[0].text + '",');
        acc = acc.nextElementSibling;
        nameCount++;
        await delay(250);
    }
}
