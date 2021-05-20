// Сюда пишем логины подписок, для которых хотим ставить лайки
const userNames = ["life_of_coders"];

// С помощью этого метода спарсил логины подписок
async function InstaSubscriptionsParse() {
    // Порядковый номер подписки
    var subNumber = 0;
    // Всего подписок
    var subCount = parseFloat(document.querySelectorAll('span[class="g47SY "]')[2].innerText);
    // Тэг подписки
    var accTag = document.getElementsByClassName('isgrP')[0].getElementsByTagName('li')[0];
    const delay = ms => new Promise(res => setTimeout(res, ms));
    console.log("Sub Count: " + subCount);

    // Когда порядковый номер подписки станет равной количеству подписок, заканчиваем цикл
    while (subNumber < subCount) {
        // Переходим на след тэг
        accTag.scrollIntoView();
        console.log('"' + accTag.getElementsByClassName('FPmhX notranslate  _0imsa ')[0].text + '",');
        // Присваиваем параметру следующий тег подписки
        accTag = accTag.nextElementSibling;
        // Инкрементируем порядковый номер тэга
        subNumber++;
        await delay(250);
    }
}

// Порядковый номер поста
var postNumber = 1;
// Задержка
const delay = ms => new Promise(res => setTimeout(res, ms));
// Параметр, куда будем присваивать тег каждого поста
var postTag = document.getElementsByTagName('article')[0];
// Количество постов, у которых активна кнопка "Мне нравится"
var oldLikes = 0;

// Функция которая ставит лайк
async function Like() {
    // Проверяем, является ли аккаунт поста тем, для которого хотим ставить лайк
    if (userNames.some(name => name === postTag.querySelectorAll('a[class*="sqdOP yWX7d"]')[0].text)) {
        // Проверка, не активна ли кнопка "нравится", если нет, то нажимаем "лайк", если да, то инкрементируем параметр oldLikes
        if (postTag.getElementsByTagName('svg')[1].ariaLabel === "Нравится") {
            postTag.getElementsByClassName('wpO6b')[1].click();
            var name = postTag.getElementsByClassName('sqdOP yWX7d')[0].text;
            console.log(`%c${name}` + ' IS LIKED!', 'color: #13a555; font-size:16px;');
            await delay(1000);
        } else {
            oldLikes++;
        }
    }
    postNumber++;
    // Присваиваем следующий пост
    postTag = postTag.nextElementSibling;
}


function oldPostLikes() {
    return oldLikes == 5;
}

console.log('%c*********************************************************', 'color: #13a555; font-size:16px;');
console.log('%c                 Like script started!', 'color: #13a555; font-size:16px;');
console.log('%c*********************************************************', 'color: #13a555; font-size:16px;');
// Если постов с актиной кнопкой "лайк" пять, то заканчивается работа цикла
while (!oldPostLikes()) {
    //Переход на следующий пост
    postTag.getElementsByClassName('wpO6b')[1].scrollIntoView({ block: "center", inline: "nearest" });
    console.log("Post number: " + postNumber + ": " + postTag.getElementsByClassName('sqdOP yWX7d')[0].text);
    Like();
    await delay(1000);
}

if (oldPostLikes()) {
    console.log('%c*******************************************************', 'color: #13a555; font-size:16px;');
    console.log('%c         New posts have been viewed.', 'color: #13a555; font-size:16px;');
    console.log('%c*******************************************************', 'color: #13a555; font-size:16px;');
}


