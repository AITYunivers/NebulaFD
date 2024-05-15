function PlatformMovement(){
	// Platform Movement object functions -- adapted to GML from Chowdren/Python
}
function pmo_set_object(pfmo, instance) {
	pfmo.instance = instance;
}
function pmo_overlaps_obstacle(pfmo) {
	return pfmo.obstacle_collision;
}
function pmo_overlaps_platform(pfmo) {
	return pfmo.platform_collision;
}
function pmo_set_position(instance, xpos, ypos) {
	with (instance){
		x = xpos;
		y = ypos;
	}
}
function pmo_update(pfmo) {
	var instance = pfmo.instance
        
    var left = pfmo.left
    var right = pfmo.right
    pfmo.left = false
	pfmo.right = false
	
	var tmp = false
        
    if not pfmo.Paused and instance != noone and instance_exists(instance)
        if right and not left
            pfmo.xVelocity += pfmo.xAccel
        if left and not right
            pfmo.xVelocity -= pfmo.xAccel
        if pfmo.xVelocity != 0 and ((not left and not right) or 
                                    (left and right))
            pfmo.xVelocity -= (pfmo.xVelocity / abs(pfmo.xVelocity
                ) * pfmo.xDecel)
            if pfmo.xVelocity <= pfmo.xDecel and pfmo.xVelocity >= 0 - pfmo.xDecel
                pfmo.xVelocity = 0

        pfmo.xVelocity = min(max(pfmo.xVelocity, 0 - pfmo.maxXVelocity),
            pfmo.maxXVelocity)
        pfmo.yVelocity = min(max(pfmo.yVelocity + pfmo.Gravity, 0 - pfmo.maxYVelocity),
            pfmo.maxYVelocity)
        xVelocity = pfmo.xVelocity + pfmo.addXVelocity
        yVelocity = pfmo.yVelocity + pfmo.addYVelocity
        pfmo.xMoveCount += abs(xVelocity)
        pfmo.yMoveCount += abs(yVelocity)
            
        while pfmo.xMoveCount > 100
            if not pmo_overlaps_obstacle(pfmo)
                pmo_set_position(instance, instance.x + xVelocity / abs(
                    xVelocity), instance.y)
            if pmo_overlaps_obstacle(pfmo)
                for (var i = 0; i < pfmo.stepUp; i++)
                    pmo_set_position(instance, instance.x, instance.y - 1)
                    if not pmo_overlaps_obstacle(pfmo)
                        break
                if pmo_overlaps_obstacle(pfmo)
                    pmo_set_position(instance, 
                        instance.x - xVelocity / abs(xVelocity),
                        instance.y + pfmo.stepUp)
                    pfmo.xVelocity = pfmo.xMoveCount = 0
            pfmo.xMoveCount -= 100
            
        while pfmo.yMoveCount > 100
            if not pmo_overlaps_obstacle(pfmo)
                pmo_set_position(instance, instance.x, 
                    instance.y + yVelocity / abs(yVelocity))
                pfmo.onGround = false
            if pmo_overlaps_obstacle(pfmo)
                pmo_set_position(instance, instance.x,
                    instance.y - yVelocity / abs(yVelocity))
                if yVelocity > 0
                    pfmo.onGround = true
                pfmo.yVelocity = pfmo.yMoveCount = 0
            if pmo_overlaps_platform(pfmo) and yVelocity > 0
                if pfmo.throughCollisionTop {
                    pmo_set_position(instance, instance.x, instance.y - 1)
                    if !pmo_overlaps_platform(pfmo)
                        pmo_set_position(instance, instance.x,
                            instance.y - yVelocity / abs(yVelocity))
                        pfmo.yVelocity = pfmo.yMoveCount = 0
                        pfmo.onGround = true
                    pmo_set_position(instance, instance.x, instance.y + 1)
				}
                else {
                    pmo_set_position(instance, instance.x,
                        instance.y - yVelocity / abs(yVelocity))
                    pfmo.yVelocity = pfmo.yMoveCount = 0
                    pfmo.onGround = true }
            pfmo.yMoveCount -= 100
        if pfmo.slopeCorrection > 0 and yVelocity >= 0
            tmp = false
            for (var i = 0; i < pfmo.slopeCorrection; i++)
                pmo_set_position(instance, instance.x,
                    instance.y + 1)
                if pmo_overlaps_obstacle(pfmo)
                    pmo_set_position(instance, instance.x,
                        instance.y - 1)
                    pfmo.onGround = true
                    tmp = true
                    break
            if tmp == false
                pmo_set_position(instance, instance.x,
                    instance.y - pfmo.slopeCorrection)
}