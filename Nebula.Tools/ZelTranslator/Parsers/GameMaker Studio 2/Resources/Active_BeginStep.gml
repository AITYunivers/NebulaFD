/// @description Manage animations
var validAnim = false;
var playingAnim = 0;
for (var i = 0; i < array_length(animations); i++){
	var animID = animations[i][1];	
	if (animation == animID){
		playingAnim = i;
		validAnim = true;
	}
}
var closest = get_closest_angle(direction, animations[playingAnim][0]);
if (validAnim) {
	sprite_index = animations[playingAnim][0][closest][0];
	XActionPoint = animations[playingAnim][0][closest][2];
	YActionPoint = animations[playingAnim][0][closest][3];
}
if (!validAnim) sprite_index = animations[0][0][0][0];
if (flash[1] > 0) {
	flash[0] += delta_time/1000000.0
	if (flash[0] >= flash[1]) {
		flash[0] = 0;
		visible = !visible;
	}
}
asset_get_index(movements[movement].typeName + "_update")(); // Update movement