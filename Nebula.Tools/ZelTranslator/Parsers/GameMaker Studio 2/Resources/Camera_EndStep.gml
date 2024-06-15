x = centerX;
y = centerY;

// Get current camera position
var camX = camera_get_view_x(camera);
var camY = camera_get_view_y(camera);
var camW = camera_get_view_width(camera);
var camH = camera_get_view_height(camera);

// Set target camera position
targetX = (x - camW/2);
targetY = (y - camH/2);

// Clamp the target to room bounds
targetX = clamp(targetX, 0, room_width - camW);
targetY = clamp(targetY, 0, room_height - camH);

// Smoothly move the camera to the target position
//camX = lerp(camX, targetX, CAM_SMOOTH);
//camY = lerp(camY, targetY, CAM_SMOOTH);
camX = targetX
camY = targetY


camera_set_view_pos(camera, camX, camY); // Set camera view

camera_set_view_size(camera, camW, camH); // Zooming
