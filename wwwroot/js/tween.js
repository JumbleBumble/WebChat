"use strict";

var dur = 10000;
if (document.title == 'Home page') {
	dur = 20000;
}
const tween1 = KUTE.fromTo(
	'#f1',
	{ path: '#f1' },
	{ path: '#ff1' },
	{ repeat: 999, duration: dur, yoyo: true }
).start();

const tween2 = KUTE.fromTo(
	'#f2',
	{ path: '#f2' },
	{ path: '#ff2' },
	{ repeat: 999, duration: dur, yoyo: true }
).start();

const tween3 = KUTE.fromTo(
	'#f3',
	{ path: '#f3' },
	{ path: '#ff3' },
	{ repeat: 999, duration: dur, yoyo: true }
).start();

const tween4 = KUTE.fromTo(
	'#f4',
	{ path: '#f4' },
	{ path: '#ff4' },
	{ repeat: 999, duration: dur, yoyo: true }
).start();