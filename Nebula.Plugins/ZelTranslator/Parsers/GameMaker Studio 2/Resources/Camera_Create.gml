centerX = 0;
centerY = 0;

targetX = 0
targetY = 0

camwidth = room_width;
camheight = room_height;

if (room_width > <CAMWIDTH>) then camwidth = <CAMWIDTH>;
if (room_height > <CAMHEIGHT>) then camheight = <CAMHEIGHT>;

//Resolution
#macro RES_W camwidth
#macro RES_H camheight
#macro RES_SCALE 1

#macro CAM_SMOOTH 0.1

// Enable views
view_enabled = true;
view_visible[0] = true;

// Create camera
camera = camera_create_view(0, 0, RES_W, RES_H);

view_set_camera(0, camera);