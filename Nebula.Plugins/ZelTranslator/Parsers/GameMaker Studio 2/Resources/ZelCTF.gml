#region ZelCTF
function ZelCTF(){
	// List of translated GML functions that are used in Fusion games
	#macro FNAF_WORLD true
}

// Utility
function obj_inst_count(obj, event) {
	var obj_count = 0;
	for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
		if global.eventInstLists[event][i].object_index == obj then obj_count++;
	}
	return obj_count;
}
function array_create_zel(size) {
	var array = [];
	for (var _i = 0; _i < size; _i++) { 
		array[_i] = []; 
	}
	return array;
}
function array_notalways(size) {
	var array = [];
	for (var i = 0; i < size; i++) {
		array[i] = true;
	}
	return array;
}
function get_closest_angle(angle, candidates) {
    var closest = -1;
    var diff = infinity;
    for (var i = 0; i < array_length(candidates); i++) {
        var current_diff = abs(angle_difference(angle, (candidates[i][1]*11.25) ));
        if (current_diff < diff) {
            closest = i;
            diff = current_diff;
        }
    }

    return closest;
}
function ini_read_zel(type, fileName, group, item){
	global.groupName_TMP = group;
	global.itemName_TMP = item;
	if (type == "value") {
		ini_open(fileName);
		variable_global_set("read_return", ini_read_real(global.groupName_TMP, global.itemName_TMP, 0));
		ini_close();
	}
	else if (type == "string") {
		
		ini_open(fileName);
		variable_global_set("read_return", ini_read_string(global.groupName_TMP, global.itemName_TMP, ""));
		ini_close();
	}
	else {
		show_message("no type specified");
	}
	return variable_global_get("read_return");
}
function ini_write_zel(type, fileName, group, item, val) {
	ini_open(fileName);
	
	if (type == "value") ini_write_real_nice(group, item, val);
	else if (type == "string") ini_write_string(group, item, val);
	
	ini_close();
}
function ini_write_real_nice(section, key, value){
	// Written by YellowAfterLife
	var v = argument2;
	var s; // :string
	if (floor(v) != v) {
	    s = string_format(argument2, 0, 16);
	    var i, n, c; // :int
	    i = string_length(s)
	    while (i > 1) {
	        c = string_ord_at(s, i)
	        if (c == 46) { // 46/"."
	            i--
	            break
	        }
	        if (c != 48) break // 48/"0"
	        i--
	    }
	    s = string_copy(s, 1, i)
	} else s = string(v)
	return ini_write_string(argument0, argument1, s)
}
function object(oname){
	if instance_exists(oname) return oname;
	return noone;
}
/*
	CONDITIONS
*/
function FacingDirection(obj, dir, paramType, val, event) {
	var returnTrue = 0;
	var ParameterInt = 0;
	var ParameterExpression = 1;
	if (obj_inst_count(obj, event) == 0) {
		if (instance_exists(obj)){
			for (var i = 0; i < instance_number(obj); i++;){
				with (instance_find(obj, i)){
					if (paramType == ParameterExpression) {
						if (floor(direction/11.25) == dir) {
							returnTrue = 1;
							array_push(global.eventInstLists[event], id);
						}
					}
					if (paramType == ParameterInt) {
						var mask = dir;
						var _dir;
						for (_dir = 0; _dir < 32; _dir++) {
							if (((1 << _dir) & mask) != 0) {
								if (floor(direction/11.25) == _dir) {
									returnTrue = 1;
									array_push(global.eventInstLists[event], id);
								}
							}
						}
					}
				}
			}	
		}
	}
	else {
		if (instance_exists(obj)){
			for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
				if global.eventInstLists[event][i].object_index == obj {
					with (global.eventInstLists[event][i]){
						if (paramType == ParameterExpression) {
							if (floor(direction/11.25) == dir) {
								returnTrue = 1;
							} else array_delete(global.eventInstLists[event],i,1);
						}
						if (paramType == ParameterInt) {
							var mask = dir;
							var _dir;
							for (_dir = 0; _dir < 32; _dir++) {
								if (((1 << _dir) & mask) != 0) {
									if (floor(direction/11.25) == _dir) {
										returnTrue = 1;
									} else array_delete(global.eventInstLists[event],i,1);
								}
							}
						}
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function MouseOver(obj, prec, notme, event) {
	var returnTrue = 0;
	if (obj_inst_count(obj, event) == 0) {
		var _inst = collision_point(mouse_x,mouse_y,obj,prec,notme)
		if _inst {
			array_push(global.eventInstLists[event], _inst);
			returnTrue = 1;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++){
			if global.eventInstLists[event][i].object_index == obj {
				if (collision_point(mouse_x,mouse_y,obj,prec,notme) == global.eventInstLists[event][i]) returnTrue = 1;
				else array_delete(global.eventInstLists[event],i,1);
			}
		}
	}
	if (returnTrue == 1) return true;
}
function ClickedObj(obj, mousebtn, prec, notme, event) {
	var returnTrue = 0;
	if mouse_check_button_pressed(mousebtn){
		if (obj_inst_count(obj, event) == 0) {
			var _inst = collision_point(mouse_x,mouse_y,obj,prec,notme)
			if _inst {
				array_push(global.eventInstLists[event], _inst);
				returnTrue = 1;
			}
		}
		else {
			for (var i = 0; i < array_length(global.eventInstLists[event]); i++){
				if global.eventInstLists[event][i].object_index == obj {
					if (collision_point(mouse_x,mouse_y,obj,prec,notme) == global.eventInstLists[event][i]) returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function CloseToWindowEdge(obj, pixels, event) { // FIX/REWORK this isn't accurate
	var returnTrue = 0;
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0;
			with (instance_find(obj, i)){
				if x <= (pixels + sprite_width/2) then returnTrue = 1;
				else if x >= (room_width - (pixels + sprite_width/2)) then returnTrue = 1;
				else if y <= (pixels + sprite_height/2) then returnTrue = 1;
				else if y >= (room_height - (pixels + sprite_height/2)) then returnTrue = 1;
			
				if (returnTrue == 1) {
					array_push(global.eventInstLists[event], id);
				}
			}
		}	
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if x <= (pixels + sprite_width/2) then returnTrue = 1;
					else if x >= (room_width - (pixels + sprite_width/2)) then returnTrue = 1;
					else if y <= (pixels + sprite_height/2) then returnTrue = 1;
					else if y >= (room_height - (pixels + sprite_height/2)) then returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function IsOverlapping(obj1, obj2, event) { // Fix to target individual obj2 instances
	var returnTrue = 0;
	if (obj_inst_count(obj1, event) == 0) {
		for (var i = 0; i < instance_number(obj1); i++;){
			// Overlapping self
			if obj1 == obj2 {
				with (instance_find(obj1, i)){
					var _inst = instance_place(x,y,obj2)
					if (id!=_inst && _inst != noone) {
						array_push(global.eventInstLists[event], id, _inst);
						returnTrue = 1;
					}
				}
			}
			else {
				// Overlapping other object
				with(instance_find(obj1, i)){
					var _inst = instance_place(x,y,obj2);
					if _inst != noone {
						array_push(global.eventInstLists[event], id, _inst);
						//show_debug_message("overlapping other object")
						returnTrue = 1;
					}
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj1 {
				if obj1 == obj2 { // Overlapping self
					with (global.eventInstLists[event][i]){
						var _inst = instance_place(x,y,obj2)
						if (id!=_inst && _inst != noone) {
							//array_push(global.eventInstLists[event], id, other.id);
							returnTrue = 1;;
						}
						else array_delete(global.eventInstLists[event],i,1);
					}
				}
				else {
					// Overlapping other object
					with(global.eventInstLists[event][i]){
						var _inst = instance_place(x,y,obj2);
						if _inst != noone {
							//array_push(global.eventInstLists[event], id, obj2.id);
							returnTrue = 1;;
						}
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function IsOverlappingBackdrop(obj, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			// Overlapping other object
			with(instance_find(obj, i)){
				if (place_meeting(x + hspeed,y,Backdrop_Obstacle) || place_meeting(x,y+vspeed,Backdrop_Obstacle)) {
					returnTrue = 1;
					array_push(global.eventInstLists[event], id);
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				// Overlapping other object
				with(global.eventInstLists[event][i]){
					if (place_meeting(x + hspeed,y,Backdrop_Obstacle) || place_meeting(x,y+vspeed,Backdrop_Obstacle)) {
						returnTrue = 1;
					} else array_delete(global.eventInstLists[event],i,1);
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function CompareAltValue(obj, variable, comparison, val, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		if (instance_exists(obj)){
			for (var i = 0; i < instance_number(obj); i++;){
				//returnTrue = 0;
				try {
					with (instance_find(obj, i)){
						switch (comparison)
						{
							case "==":
								if AlterableValues[variable] == val then {returnTrue = 1;
									array_push(global.eventInstLists[event], id);}
							break;
				
							case "!=":
								if AlterableValues[variable] != val then {returnTrue = 1;
									array_push(global.eventInstLists[event], id);}
							break;
				
							case "<=":
								if AlterableValues[variable] <= val then {returnTrue = 1;
									array_push(global.eventInstLists[event], id);}
							break;
				
							case "<":
								if AlterableValues[variable] < val then {returnTrue = 1;
									array_push(global.eventInstLists[event], id);}
							break;
				
							case ">=":
								if AlterableValues[variable] >= val then {returnTrue = 1;
									array_push(global.eventInstLists[event], id);}
							break;
				
							case ">":
								if AlterableValues[variable] > val {returnTrue = 1;
									array_push(global.eventInstLists[event], id);}
							break;
						
							default:
								throw ("Invalid comparison operator for CompareAltValue()!");
							break;
						}
			
						if (returnTrue == 1) {
							//array_push(global.eventInstLists[event], id);
						}
					}
				} catch(_ex){}
			}	
		}
	}
	else {
		if (instance_exists(obj)){
			for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
				if global.eventInstLists[event][i].object_index == obj {
					try {
						with (global.eventInstLists[event][i]){
							switch (comparison)
							{
								case "==":
									if AlterableValues[variable] == val then returnTrue = 1;
									else array_delete(global.eventInstLists[event],i,1);
								break;
				
								case "!=":
									if AlterableValues[variable] != val then returnTrue = 1;
									else array_delete(global.eventInstLists[event],i,1);
								break;
				
								case "<=":
									if AlterableValues[variable] <= val then returnTrue = 1;
									else array_delete(global.eventInstLists[event],i,1);
								break;
				
								case "<":
									if AlterableValues[variable] < val then returnTrue = 1;
									else array_delete(global.eventInstLists[event],i,1);
								break;
				
								case ">=":
									if AlterableValues[variable] >= val then returnTrue = 1;
									else array_delete(global.eventInstLists[event],i,1);
								break;
				
								case ">":
									if AlterableValues[variable] > val then returnTrue = 1;
									else array_delete(global.eventInstLists[event],i,1);
								break;
							
								default:
									throw ("Invalid comparison operator for CompareAltValue()!");
								break;
							}
			
							if (returnTrue == 1) {
								//array_push(global.eventInstLists[event], id);
							}
						}
					} catch(_ex){}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function CompareXorYPos(obj, XorY, comparison, val, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0;
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					switch (comparison)
					{
						case "==":
							if XorY == "x" && self.x == val then returnTrue = 1;
							else if XorY == "y" && self.y == val then returnTrue = 1;
						break;
				
						case "!=":
							if XorY == "x" && self.x != val then returnTrue = 1;
							else if XorY == "y" && self.y != val then returnTrue = 1;
						break;
				
						case "<=":
							if XorY == "x" && self.x <= val then returnTrue = 1;
							else if XorY == "y" && self.y <= val then returnTrue = 1;
						break;
				
						case "<":
							if XorY == "x" && self.x < val then returnTrue = 1;
							else if XorY == "y" && self.y < val then returnTrue = 1;
						break;
				
						case ">=":
							if XorY == "x" && self.x >= val then returnTrue = 1;
							else if XorY == "y" && self.y >= val then returnTrue = 1;
						break;
				
						case ">":
							if XorY == "x" && self.x > val then returnTrue = 1;
							else if XorY == "y" && self.y > val then returnTrue = 1;
						break;
					}
			
					if (returnTrue == 1) {
						array_push(global.eventInstLists[event], id);
					}
				}
			}
		}	
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					switch (comparison)
					{
						case "==":
							if XorY == "x" && self.x == val then returnTrue = 1;
							else if XorY == "y" && self.y == val then returnTrue = 1;
							else array_delete(global.eventInstLists[event],i,1);
						break;
				
						case "!=":
							if XorY == "x" && self.x != val then returnTrue = 1;
							else if XorY == "y" && self.y != val then returnTrue = 1;
							else array_delete(global.eventInstLists[event],i,1);
						break;
				
						case "<=":
							if XorY == "x" && self.x <= val then returnTrue = 1;
							else if XorY == "y" && self.y <= val then returnTrue = 1;
							else array_delete(global.eventInstLists[event],i,1);
						break;
				
						case "<":
							if XorY == "x" && self.x < val then returnTrue = 1;
							else if XorY == "y" && self.y < val then returnTrue = 1;
							else array_delete(global.eventInstLists[event],i,1);
						break;
				
						case ">=":
							if XorY == "x" && self.x >= val then returnTrue = 1;
							else if XorY == "y" && self.y >= val then returnTrue = 1;
							else array_delete(global.eventInstLists[event],i,1);
						break;
				
						case ">":
							if XorY == "x" && self.x > val then returnTrue = 1;
							else if XorY == "y" && self.y > val then returnTrue = 1;
							else array_delete(global.eventInstLists[event],i,1);
						break;
					}
			
					if (returnTrue == 1) {
						//array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function PickOneOf(obj, event) {
	var returnTrue = 0;
	var objarray = [];
	if (obj_inst_count(obj, event) == 0) {
		objarray = [];
		for (var i = 0; i < instance_number(obj); i++;){
			if instance_find(obj, i).object_index == obj {
				array_push(objarray, instance_find(obj, i).id);
				returnTrue = 1;
			}
		}
		if (returnTrue == 1) {
			array_push(global.eventInstLists[event], objarray[irandom(array_length(objarray)-1)]);
		}
	}	
	else {
		objarray = [];
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				array_push(objarray, instance_find(obj, i).id);
				array_delete(global.eventInstLists[event], i, 1);
				returnTrue = 1;
			}
		}
		if (returnTrue == 1) {
			array_push(global.eventInstLists[event], objarray[irandom(array_length(objarray)-1)]);
		}
	}
	if (returnTrue == 1) return true;
}
function IsStopped(obj, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					if Movement.moving == false then returnTrue = 1;
					if (returnTrue == 1) {
						array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if Movement.moving == false then returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
					if (returnTrue == 1) {
						//array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function IsVisible(obj, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					if visible == true then returnTrue = 1;
					if (returnTrue == 1) {
						array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if visible == true then returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
					if (returnTrue == 1) {
						//array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function IsAnimPlaying(obj, animID, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					if animation == animID then returnTrue = 1;
					if (returnTrue == 1) {
						array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if animation == animID then returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
					if (returnTrue == 1) {
						//array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function CompareAnimFrame(obj, frame, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					if image_index == frame then returnTrue = 1;
					if (returnTrue == 1) {
						array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if image_index == frame then returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
					if (returnTrue == 1) {
						//array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function HasAnimFinished(obj, animID, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		for (var i = 0; i < instance_number(obj); i++;){
			returnTrue = 0
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					if (animation == animID && image_index > image_number - 1) then returnTrue = 1;
					if (returnTrue == 1) {
						array_push(global.eventInstLists[event], id);
					}
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if (animation == animID && image_index > image_number - 1) then returnTrue = 1;
					else array_delete(global.eventInstLists[event],i,1);
					if (returnTrue == 1) {
						//array_push(global.eventInstLists[event], id);
						return true;
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}
function NumInZone(obj, x1, y1, x2, y2, event) {
	var _objCount = 0
		for (var i = 0; i < instance_number(obj); i++;){
			if instance_find(obj, i).object_index == obj {
				with (instance_find(obj, i)){
					if (x >= x1 && x <= x2 && y >= y1 && y <= y2) then _objCount++
				}
			}
		}
	return _objCount
}

// Counters
function CompareCounter(obj, comparison, val, event) {
	var returnTrue = 0
	if (obj_inst_count(obj, event) == 0) {
		if (instance_exists(obj)){
			for (var i = 0; i < instance_number(obj); i++;){
				returnTrue = 0;
				if instance_find(obj, i).object_index == obj {
					with (instance_find(obj, i)){
						switch (comparison)
						{
							case "==":
								if cntrvalue == val then returnTrue = 1;
							break;
				
							case "!=":
								if cntrvalue != val then returnTrue = 1;
							break;
				
							case "<=":
								if cntrvalue <= val then returnTrue = 1;
							break;
				
							case "<":
								if cntrvalue < val then returnTrue = 1;
							break;
				
							case ">=":
								if cntrvalue >= val then returnTrue = 1;
							break;
				
							case ">":
								if cntrvalue > val then returnTrue = 1;
							break;
						}
			
						if (returnTrue == 1) {
							array_push(global.eventInstLists[event], id);
						}
					}
				}
			}	
		}
	}
	else {
		if (instance_exists(obj)){
			for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
				if global.eventInstLists[event][i].object_index == obj {
					with (global.eventInstLists[event][i]){
						switch (comparison)
						{
							case "==":
								if cntrvalue == val then returnTrue = 1;
								else array_delete(global.eventInstLists[event],i,1);
							break;
				
							case "!=":
								if cntrvalue != val then returnTrue = 1;
								else array_delete(global.eventInstLists[event],i,1);
							break;
				
							case "<=":
								if cntrvalue <= val then returnTrue = 1;
								else array_delete(global.eventInstLists[event],i,1);
							break;
				
							case "<":
								if cntrvalue < val then returnTrue = 1;
								else array_delete(global.eventInstLists[event],i,1);
							break;
				
							case ">=":
								if cntrvalue >= val then returnTrue = 1;
								else array_delete(global.eventInstLists[event],i,1);
							break;
				
							case ">":
								if cntrvalue > val then returnTrue = 1;
								else array_delete(global.eventInstLists[event],i,1);
							break;
						}
			
						if (returnTrue == 1) {
							//array_push(global.eventInstLists[event], id);
						}
					}
				}
			}
		}
	}
	if (returnTrue == 1) return true;
}

// Joystick 2 Object
function gp_anybutton(device){
	for ( var b = gp_face1; b <= gp_stickr; b++ ) {
	    if (gamepad_button_check(device, b)) return b;
	}
	return gp_face1
}
/*
	ACTIONS
*/
function JumpToFrame(num) {
	var _count = 0;
	var _len = string_length("rm0_");
	if (num >= 10) _len++;
	if (num >= 100) _len++ 
	while (true) {
		if (asset_get_type(_count) == asset_room) {
			if (string_copy(room_get_name(_count),1,_len) == "rm" + string(num) + "_") {
				room_goto(_count);
			}
		}
		_count++;
	}
}
function SetDirection(obj, dir, paramType, event) {
	var ParameterInt = 0
	var ParameterExpression = 1
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			with (obj) {
				if (paramType == ParameterInt) set_direction_int(dir);
				if (paramType == ParameterExpression) set_direction(floor((dir mod 32) * 11.25));
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if (paramType == ParameterInt) set_direction_int(dir);
					if (paramType == ParameterExpression) set_direction(floor((dir mod 32) * 11.25));
				}
			}
		}	
	}
}
function SetGeneralValue(obj, variable, operation, val, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			for (var i = 0; i < instance_number(obj); i++) {
				var _inst = instance_find(obj,i)
				switch (operation)
				{
					case "=":
						variable_instance_set(_inst, variable, val);
					break;
				
					case "+=":
						variable_instance_set(_inst, variable, variable_instance_get(_inst, variable) + val);
					break;
				
					case "-=":
						variable_instance_set(_inst, variable, variable_instance_get(_inst, variable) - val);
					break;
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					switch (operation)
					{
						case "=":
							variable_instance_set(id, variable, val);
						break;
				
						case "+=":
							variable_instance_set(id, variable, variable_instance_get(id, variable) + val);
						break;
				
						case "-=":
							variable_instance_set(id, variable, variable_instance_get(id, variable) - val);
						break;
					}
				}
			}
		}	
	}
}
function SetAltValue(obj, variable, operation, val, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			switch (operation)
			{
				case "=":
					obj.AlterableValues[variable] = val;
				break;
				
				case "+=":
					obj.AlterableValues[variable] += val;
				break;
				
				case "-=":
					obj.AlterableValues[variable] -= val;
				break;
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					switch (operation)
					{
						case "=":
							AlterableValues[variable] = val;
						break;
				
						case "+=":
							AlterableValues[variable] += val;
						break;
				
						case "-=":
							AlterableValues[variable] -= val;
						break;
					}
				}
			}
		}	
	}
}
function SetAltString(obj, variable, val, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			obj.AlterableStrings[variable] = val;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					AlterableStrings[variable] = val;
				}
			}
		}	
	}
}
function SetFlag(obj, flag, val, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			for (var i = 0; i < instance_number(obj); i++) {
				var _inst = instance_find(obj, i)
				if (val == "toggle") {
					_inst.Flags[flag] = !_inst.Flags[flag];
				} else {
					_inst.Flags[flag] = val;
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if (val == "toggle") {
						Flags[flag] = !Flags[flag];
					} else {
						Flags[flag] = val;
					}
				}
			}
		}	
	}
}
function SetVisibility(obj, visiblebool, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			with (obj) {
				visible = visiblebool;
				flash = [0,0];
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					visible = visiblebool;
					flash = [0,0];
				}
			}
		}	
	}
}
function FlashObject(obj, flashTime, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			with (obj) {
				visible = false;
				flash = [0,flashTime/1000.0];
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					visible = false;
					flash = [0,flashTime/1000.0];
				}
			}
		}	
	}
}
function DestroyObject(obj, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then instance_destroy(obj);
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					instance_destroy(id);
				}
			}
		}	
	}
}
function CreateObject(obj, xpos, ypos, event, Layer, typeParent = 0, parentObj = noone) { // Fix to target individual obj2 instances
	if typeParent == 2 {
		if (obj_inst_count(parentObj, event) == 0){
			for (var i = 0; i < instance_number(parentObj); i++) {
				var parInst = instance_find(parentObj,i);
				var inst = instance_create_depth(parInst.x + xpos, parInst.y + ypos, parInst.depth, obj);
				inst.depth--;
				array_push(global.eventInstLists[event], inst);
			}
		}
		else {
			for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
				if global.eventInstLists[event][i].object_index == parentObj {
					var parInst = global.eventInstLists[event][i];
					var inst = instance_create_depth(parInst.x + xpos, parInst.y + ypos, parInst.depth, obj);
					inst.depth--;
					array_push(global.eventInstLists[event], inst);
				}
			}
		}	
	}
	else {
		var setLayer = (array_length(layer_get_all())-1) - (Layer);
		var setDepth = layer_get_depth(setLayer);
		var inst = instance_create_depth(xpos, ypos, setDepth, obj);
		inst.depth--;
		array_push(global.eventInstLists[event], inst);
	}
}
function MoveToLayer(obj, Layer, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			obj.layer = (array_length(layer_get_all())-1) - (Layer);
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					layer = (array_length(layer_get_all())-1) - (Layer);;
				}
			}
		}	
	}
}
function SetPosition(obj, xpos, ypos, event, typeParent = 0, parentObj = noone) {
	// Find parentObj instance
	if (obj_inst_count(parentObj, event) > 0) {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if (global.eventInstLists[event][i].object_index == parentObj) parentObj = global.eventInstLists[event][i];
		}
	}
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			if typeParent == 2 {
				if obj == parentObj.object_index {
					obj.x += xpos;
					obj.y += ypos;
				}
				else {
					obj.x = parentObj.x + xpos;
					obj.y = parentObj.y + ypos;
				}
			}
			else {
				obj.x = xpos;
				obj.y = ypos;
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if typeParent == 2 {
						if object_index == parentObj.object_index {
							x += xpos;
							y += ypos;
						}
						else {
							x = parentObj.x + xpos;
							y = parentObj.y + ypos;
						}
					}
					else {
						x = xpos;
						y = ypos;
					}
				}
			}
		}	
	}
}
function SetXPos(obj, xpos, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			obj.x = xpos;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					self.x = xpos;
				}
			}
		}	
	}
}
function SetYPos(obj, ypos, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			obj.y = ypos;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					self.y = ypos;
				}
			}
		}	
	}
}
function SetMoving(obj, movingbool, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then obj.Movement.moving = movingbool;
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					Movement.moving = movingbool;
				}
			}
		}	
	}
}
function SetSpd(obj, spdval, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			with(obj) {
				movements[movement].spd = spdval;
				movements[movement].maxSpd = spdval;
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					movements[movement].spd = spdval;
					movements[movement].maxSpd = spdval;
				}
			}
		}	
	}
}
function LookAt(obj, xpos, ypos, event) { // Fix to target individual obj2 instances
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			with(obj) {
				set_direction(round(point_direction(x, y, xpos, ypos)/ 11.25));
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					set_direction(floor(point_direction(x, y, xpos, ypos)/ 11.25));
				}
			}
		}	
	}
}
function SetAlphaCoef(obj, val, event) {
	var value = 1 - (val/256.0) 
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then obj.image_alpha = clamp(value,0,1);
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					image_alpha = clamp(value,0,1);
				}
			}
		}	
	}
}
function SetSemiTrans(obj, val, event) { // For compatibility
	SetAlphaCoef(obj,val*2,event);
}
function ChangeAnimSequence(obj, animID, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then obj.animation = animID;
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					animation = animID;
				}
			}
		}	
	}
}
function SetAngle(obj, val, event) {
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then obj.image_angle = val;
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					image_angle = val;
				}
			}
		}	
	}
}
// Counters
function SetCounter(obj, val, event) {
	if (obj == noone || obj == -4) exit;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then obj.cntrvalue = clamp(val, obj.minval, obj.maxval);
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					cntrvalue = clamp(val, minval, maxval);
				}
			}
		}	
	}
}

// Strings
function SetAlterableString(obj, text, event) {
	if (obj == noone || obj == -4) exit;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			for (var i = 0; i < instance_number(obj); i++){
				with(instance_find(obj,i)) {
					paragraphs[0] = text;
					paragraph = 0;
					altString = true;
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					paragraphs[0] = text;
					paragraph = 0;
					altString = true;
				}
			}
		}	
	}
}
function DisplayAlterableString(obj, event) {
	if (obj == noone || obj == -4) exit;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			for (var i = 0; i < instance_number(obj); i++){
				with(instance_find(obj,i)) {
					paragraph = 0;
					altString = true;
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					paragraph = 0;
					altString = true;
				}
			}
		}	
	}
}
function SetParagraph(obj, para, event, prevNext = "") {
	if (obj == noone || obj == -4) exit;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) {
			for (var i = 0; i < instance_number(obj); i++){
				with(instance_find(obj,i)) {
					if (prevNext == "prev") paragraph = clamp(paragraph-1, 1, array_length(paragraphs)-1);
					else if (prevNext == "next") paragraph = clamp(paragraph+1, 1, array_length(paragraphs)-1);
					else paragraph = clamp(para+1, 1, array_length(paragraphs)-1);;
					altString = false;
				}
			}
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if (prevNext == "prev") paragraph = clamp(paragraph-1, 1, array_length(paragraphs)-1);
					else if (prevNext == "next") paragraph = clamp(paragraph+1, 1, array_length(paragraphs)-1);
					else paragraph = clamp(para+1, 1, array_length(paragraphs)-1);;
					altString = false;
				}
			}
		}	
	}
}

/*
	EXPRESSIONS
*/
// Special
function LeftStr(str, num) {
	return string_copy(str, 0, min(num,string_length(str)));
}
function MidStr(str, index, num) {
	return string_copy(str, index+1, min(num, string_length(str)));
}
function RightStr(str, num) {
	return string_copy(str,string_length(str)-(num-1), min(num, string_length(str)-(num-1)));
}
function Find(str, substr, index) {
	return string_pos_ext(str, substr, index+1);
}
function ReverseFind(str, substr, index) {
	return string_last_pos_ext(str, substr, index+1);
}
function Global(type, val) {
	return variable_global_get("Global" + (type == "val" ? "Values" : "Strings"))[floor(val)];
}
// Sound
function ChannelVolume(channelID) {
	return variable_global_get("channel" + string(channelID)) * 100.0;
}
// Actives
function GetGeneralValue(obj, val, type, event) {
	if (obj == noone && type == "val") return 0;
	if (obj == noone && type == "str") return "";
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then return variable_instance_get(instance_find(obj,0), val);
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return variable_instance_get(global.eventInstLists[event][i], val);
				}
			}
		}	
	}
	if type == "val" return 0;
	if type == "str" return "";
}
function Alterable(obj, type, event, val) {
	if (obj == noone && type == "val") return 0;
	if (obj == noone && type == "str") return "";
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0 && type == "val") then return obj.AlterableValues[val];
		else if (instance_number(obj) > 0 && type == "str") then return obj.AlterableStrings[val];
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if type == "val" return AlterableValues[val];
					else if type == "str" return AlterableStrings[val];
				}
			}
		}	
	}
	if type == "val" return 0;
	if type == "str" return "";
}
function Flag(obj, event, val) {
	if (obj == noone) return false;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then return obj.Flags[val];
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return Flags[val];
				}
			}
		}	
	}
	return false;
}

// Counters
function CounterValue(obj, event) {
	if (obj == noone) return 0;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0) then return obj.cntrvalue;
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return cntrvalue;
				}
			}
		}	
	}
	return 0; // FIX/REWORK - NOTE: Not sure if this is accurate return if no value is retrieved...
}
function CounterMinMax(obj, minmax, event) {
	if (obj == noone) return 0;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0){ 
			if minmax == "min" return obj.minval;
			else if minmax == "max" return obj.maxval;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					if minmax == "min" return minval;
					else if minmax == "max" return maxval
				}
			}
		}	
	}
	return 0;
}
// Strings
function AlterableString(obj, event) {
	if (obj == noone) return "";
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0){ 
			return obj.str;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return str;
				}
			}
		}	
	}
	return "";
}
function ParagraphText(obj, event, para) {
	if (obj == noone) return "";
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0){ 
			return obj.paragraphs[clamp(para,1,array_length(obj.paragraphs)-1)];
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return paragraphs[clamp(para,1,array_length(paragraphs)-1)];
				}
			}
		}	
	}
	return "";
}
function StringVal(obj, event) {
	if (obj == noone) return 0;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0){ 
			return real(string_digits(obj.str));
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return real(string_digits(str));
				}
			}
		}	
	}
	return 0;
}
function NPara(obj, event) {
	if (obj == noone) return 0;
	if (obj_inst_count(obj, event) == 0){
		if (instance_number(obj) > 0){ 
			return array_length(obj.paragraphs)-1;
		}
	}
	else {
		for (var i = 0; i < array_length(global.eventInstLists[event]); i++;){
			if global.eventInstLists[event][i].object_index == obj {
				with (global.eventInstLists[event][i]){
					return array_length(paragraphs)-1
				}
			}
		}	
	}
	return 0;
}
// OS object
function ComputerName(){
	var _vname = "";
	switch (os_type){
		case os_windows:
			_vname = "COMPUTERNAME";
		break;
		case os_macosx:
			var _tmp = environment_get_variable("COMPUTERNAME");
			if (_tmp == undefined || _tmp == "") _tmp = environment_get_variable("HOSTNAME");
			return _tmp;
			os_get_info()
		break;
		case os_linux:
			_vname = "HOSTNAME";
		break;
	}
	return environment_get_variable(_vname);
}
// Joystick 2 Object
function gp_const(btn) {
	switch (btn) {
		case 1:
			return gp_face1;
		break;
		case 2:
			return gp_face2;
		break;
		case 3:
			return gp_face3;
		break;
		case 4:
			return gp_face4;
		break;
		case 5:	
			return gp_shoulderl;
		break;
		case 6:
			return gp_shoulderr;
		break;
		case 7:
			return gp_select;
		break;
		case 8:
			return gp_start;
		break;
		case 9:
			return gp_stickl;
		break;
		case 10:
			return gp_stickr;
		break;
	}
}
function RawX(device) {
	return (gamepad_axis_value(device - 1, gp_axislh) * 1000.0)
}
function RawY(device) {
	return (gamepad_axis_value(device - 1, gp_axislv) * 1000.0)
}
#endregion
#region KcIni object
function KcIni(){
	if (variable_global_get("kcini") != true){
		show_debug_message("[ KcIni object -- Written by AITYunivers; adapted to GML by KewlSokudo ]");
		variable_global_set("kcini", true);
	}
	save = false;
	iniCurrentGroup = "";
	iniCurrentItem = "";
	if (iniName == GetFileName(iniName) && !FNAF_WORLD){
		iniName = environment_get_variable("APPDATA") + "\\MMFApplications\\" + iniName;
	}
	strings = undefined;
	currentFileName = undefined;
	
	show_debug_message("[" + object_name + "] KcIni successfully initialized!");
}
/*
	Utilities
*/
function WriteAllLinesOld(fileName, stringArray){ // Writes all array indexes line by line
	var file = file_text_open_write(fileName);
	var str = "";
	for (var i = 0; i < array_length(stringArray); i++)
	{
		str += stringArray[i];
		if (i < array_length(stringArray) - 1 && string_char_at(stringArray[i], string_length(stringArray[i])) != "\n") str += "\n";
	}
	file_text_write_string(file,str);
	file_text_close(file);
}
function WriteAllLines(fileName, stringArray){
	var buff = buffer_create(0, buffer_grow, 1);
	var str = "";
	for (var i = 0; i < array_length(stringArray); i++)
	{
		str += stringArray[i];
		if (i < array_length(stringArray) - 1 && string_char_at(stringArray[i], string_length(stringArray[i])) != "\n") str += "\n";
	}
	buffer_seek(buff, buffer_seek_start, 0);
	buffer_write(buff, buffer_string, str);
	buffer_save(buff, fileName);
	buffer_delete(buff);
}
function ReadAllLinesOld(fileName){ // Reads all lines of a file into array indexes
	var file = file_text_open_read(fileName);
    var str = "";
	var i = 0;
	var arr = []
    while (file_text_eof(file) == false) {
        arr[i] = file_text_read_string(file);
		file_text_readln(file);
		i++;
    }
    file_text_close(file);
	return arr
}
function ReadAllLines(fileName){ // Reads all lines of a file into array indexes
	show_debug_message("About to load " + fileName)
	var file = buffer_load(fileName);
	show_debug_message("File loaded into buffer!")
	var s = buffer_read(file, buffer_string);
	buffer_delete(file);
	var arr = [];
	var line = 0;
	var str = "";
	for (var i = 1; i < string_length(s)+1; i++){
		var char = string_copy(s,i,1);
		var chars2 = string_copy(s,i,2);
		if (chars2 == "\r\n" || chars2 == "\n\r") {
			arr[line] = str;
			str = "";
			line++;
			i++;
		}
		else if (char == "\n" || char == "\r") {
			arr[line] = str;
			str = "";
			line++;
			//i++;
		}
		else {
			if (char != "\n" && char != "\r") str += char;
		}
	}
	arr[line] = str; // Append final line when EoF reached
	
	return arr
}
function GetFileName(str){ // Returns only name of a given file path
	var pos = string_last_pos("\\",str)
	var fname = string_copy(str, pos+1, string_length(str) - pos + 1)
	
	pos = string_last_pos("/",fname);
	fname = string_copy(fname, pos+1, string_length(fname) - pos + 1)
	
	return fname;
}
/*
	ACTIONS
*/
function SetCurrentGroup(groupname){
	iniCurrentGroup = groupname;
}
function SetCurrentItem(itemname){
	iniCurrentItem = itemname;
}
function SetValue(value){
	writePrivateProfileString(iniCurrentGroup, iniCurrentItem, string(floor(value)), iniName);
	save = true;
}
function SavePosition(obj){ // (FIX/REWORK to work with multiple instances)
	var s = string(obj.x) + "," + string(obj.y);
	var item = "pos." + obj.object_name;
	writePrivateProfileString(iniCurrentGroup, item, s, iniName);
	save = true;
}
function LoadPosition(obj){ // (FIX/REWORK to work with multiple instances)
	var item = "pos." + obj.object_name;
	var s = getPrivateProfileString(iniCurrentGroup, item, "X", iniName);
	var compare = string_length("X") - string_length(s)
	if (compare != 0)
	{
		var virgule = string_pos(",", s);
		var left = string_copy(s, 0, virgule);
		var right = string_copy(s, virgule + 1, string_length(s) - (virgule + 1));
		obj.x = real(left);
		obj.y = real(right);
	}
}
function SetString(str){
	writePrivateProfileString(iniCurrentGroup, iniCurrentItem, str, iniName);
	save = true;
}
function SetCurrentFile(name){
	iniName = name;
	if (iniName == GetFileName(iniName) && !FNAF_WORLD){
		iniName = environment_get_variable("APPDATA") + "\\MMFApplications\\" + iniName;
	}
}
function SetValueItem(item, val){
	writePrivateProfileString(iniCurrentGroup, item, string(floor(val)), iniName);
	save = true;
}
function SetValueGroupItem(group, item, val){
	writePrivateProfileString(group, item, string(floor(val)), iniName);
	save = true;
}
function SetStringItem(item, str){
	writePrivateProfileString(iniCurrentGroup, item, str, iniName);
	save = true;
}
function SetStringGroupItem(group, item, str){
	writePrivateProfileString(group, item, str, iniName);
	save = true;
}
function DeleteItem(item){
	deleteItem(iniCurrentGroup, item, iniName);
	save = true;
}
function DeleteGroupItem(group, item){
	deleteItem(group, item, iniName);
	save = true;
}
function DeleteGroup(group){
	deleteGroup(group, iniName);
	save = true;
}
/*
	EXPRESSIONS
*/
function Exp_GetValue(iniObj){
	with(iniObj){
		var str = getPrivateProfileString(iniCurrentGroup, iniCurrentItem, "", iniName);
		if (str != "") return real(str);
		return 0;
	}
}
function Exp_GetString(iniObj){
	with(iniObj){
		return getPrivateProfileString(iniCurrentGroup, iniCurrentItem, "", iniName);
	}
}
function Exp_GetValueItem(iniObj, item){
	with(iniObj){	
		var str = getPrivateProfileString(iniCurrentGroup, item, "", iniName);
		if (str != "") return real(str);
		return 0;
	}
}
function Exp_GetValueGroupItem(iniObj, group, item){
	with(iniObj){
		var str = getPrivateProfileString(group, item, "", iniName)
		if (str != "") return real(str);
		return 0;
	}
}
function Exp_GetStringItem(iniObj, item){
	with(iniObj){
		return getPrivateProfileString(iniCurrentGroup, item, "", iniName);
	}
}
function Exp_GetStringGroupItem(iniObj, group, item){
	with(iniObj){
		return getPrivateProfileString(group, item, "", iniName);
	}
}
/*
	CIni
*/
function loadIni(fileName) // void
{
	if (currentFileName != undefined && string_lower(currentFileName) == string_lower(fileName))
		return;
	
	saveIni();
	if (file_exists(fileName)){
		strings = ReadAllLines(fileName);
	}
	else{
		strings = [];
	}
	currentFileName = fileName;
}

function saveIni() // void
{
	if (strings != undefined && currentFileName != undefined){
		WriteAllLines(currentFileName, strings);
	}
	save = false;
}

function findSection(sectionName) // int
{
	show_debug_message(strings);
	for (var l = 0; l < array_length(strings); l++)
	{
		var s = strings[l];
		if (string_char_at(s,1) == "[")
		{
			var last = string_last_pos("]", s)-1;
			if (last >= 1 && string_lower(string_copy(s, 2, last - 1)) == string_lower(sectionName)){
				show_debug_message("[" + object_name + "]" + " Found section \"" + sectionName + "\"");
				return l;
			}
		}
	}
	
	show_debug_message("[" + object_name + "]" + " COULD NOT find section \"" + sectionName + "\"");
	return -1;
}

function findKey(l, keyName) // int
{
	for (; l < array_length(strings); l++)
	{
		var s = strings[l];
		if (string_copy(s,0,1) == "["){
			return -1;
		}
			
		var last = string_pos("=", s);
		if (last >= 0 && string_copy(s, 0, last-1) == keyName){
			show_debug_message("[" + object_name + "]" + " Found key \"" + keyName + "\"");
			return l;
		}
	}
	show_debug_message("[" + object_name + "]" + " COULD NOT find key \"" + keyName + "\"");
}

function getPrivateProfileString(sectionName, keyName, defaultString, fileName) // string
{
	loadIni(fileName);
	
	var l = findSection(sectionName);
	if (l >= 0)
	{
		l = findKey(l + 1, keyName);
		if (l >= 0)
		{
			var s = strings[l]
			var last = string_pos("=",s);
			
			var start;
			for (start = last + 1; start < string_length(s); start++){
				if (string_copy(s, start, 1) != " "){
					break;
				}
			}
					
			var End;
			for (End = string_length(s); End > start; End--){
				if (string_copy(s, End, 1) != " "){
					break;
				}
			}
			if (End+1 > start){
				return string_copy(s, start, (End - start)+1);
			}
		}
	}
	return defaultString
}

function writePrivateProfileString(sectionName, keyName, name, fileName) // void
{
	loadIni(fileName);
	
	var s;
	var section = findSection(sectionName);
	if (section < 0)
	{
		s = "[" + sectionName + "]";
		array_push(strings, s);
		s = keyName + "=" + name;
		array_push(strings, s);
		return;
	}
	
	var key = findKey(section + 1, keyName);
	if (key >= 0)
	{
		s = keyName + "=" + name;
		strings[key] = s;
		return;
	}
	
	for (key = section + 1; key < array_length(strings); key++)
	{
		s = strings[key];
		if (string_copy(s, 0, 1) == "[")
		{
			s = keyName + "=" + name;
			array_insert(strings, key, s);
			return;
		}
	}
	s = keyName + "=" + name;
	array_push(strings, s);
}

function deleteItem(group, item, iniName) // void
{
	loadIni(iniName);
	
	var s = findSection(group);
	if (s >= 0)
	{
		var k = findKey(s + 1, item);
		if (k >= 0){
			array_delete(strings, k, 1);
		}
	}
}

function deleteGroup(group, iniName) // void
{
	loadIni(iniName);
	
	var s = findSection(group);
	if (s >= 0)
	{
		array_delete(strings, s, 1);
		while (true)
		{
			if (s >= array_length(strings) || string_copy(strings[s], 0, 1) == "["){
				break;
			}
			
			array_delete(strings, s, 1);
		}
	}
}
#endregion
#region Movements
function get_pixels(value) { return value / 8.0 };
function dt(frameTime = -1){ 
	if (frameTime == -1) return Frame.MoveTimer * delta_time/1000000;
	return frameTime * delta_time/1000000;
};
function lock_dir(numOfAng) {
	var _div = 360 / numOfAng;
	direction = round(round(direction / _div) * _div)
}
function set_direction(_dir, angNum = 32) {
	direction = _dir;
	lock_dir(angNum); // Lock to "# of Directions" property
}
function set_direction_int(_dir, angNum = 32) {
	var _div = 360 / angNum;
	direction = floor(get_direction(_dir) * _div);
	lock_dir(angNum); // Lock to "# of Directions" property
}
function get_direction(_dir) {
	// Random within 32 directions
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~
	if (_dir == 0 || _dir == -1) return irandom(32);
	
	// Count the number of wanted directions
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	var loop;
	var found = 0;
	var count = 0;
	var dirShift = _dir;
	for (loop = 0; loop < 32; loop++)
	{
		if ((dirShift & 1) != 0)
		{
			count++;
			found = loop;
		}
		dirShift=(dirShift>>1)&0x7FFFFFFF;
	}
	
	// One direction?
	// ~~~~~~~~~~~~~~
	if (count == 1) return found;
	
	// Random
	// ~~~~~~
	count = irandom(count);
	dirShift = _dir;
	for (loop = 0; loop < 32; loop++)
	{
		if ((dirShift & 1) != 0)
		{
			count--;
			if (count < 0) return loop;
		}
		dirShift=(dirShift>>1)&0x7FFFFFFF; 
	}
	return 0;
}

#region CMoveStatic - Stopped
function CMoveStatic_update() {

};
#endregion

#region CMoveGeneric - Eight Directions

#endregion

#region CMovePlatform - Platform

#endregion

#region CMoveBall - Bouncing Ball
function CMoveBall_start() {
	var mvt = movements[movement];
	mvt.spd = mvt.maxSpd;
	speed = get_pixels(mvt.spd) * dt();
}
function CMoveBall_update() {
	var mvt = movements[movement];
	// Decelerate
	if (mvt.deceleration > 0) {
		mvt.spd = max(0, mvt.spd - (get_pixels(mvt.deceleration) * dt()))
	}
	speed = get_pixels(mvt.spd) * dt();
}
function CMoveBall_bounce(event) {
	var collide = false;
	var str = ""
	// Search for "relevant" active collisions
	for (var i = 0; i < array_length(global.eventInstLists[event]); i++){
		var _inst = global.eventInstLists[event][i];
		if place_meeting(x + hspeed,y,_inst) {
			hspeed = -hspeed;
			collide = true;
		}
		if place_meeting(x,y + vspeed,_inst) {
			vspeed = -vspeed
			collide = true;
		}
	}
	if (!collide) { // Try backdrop collisions
		if place_meeting(x,y + vspeed,Backdrop_Obstacle) {
			vspeed = -vspeed
			collide = true;
			str += "v"
		}
		if place_meeting(x + hspeed,y,Backdrop_Obstacle) {
			hspeed = -hspeed;
			collide = true;
			str += "h"
		}
		
	}
	if (!collide) { // Fallback
		hspeed = -hspeed;
		vspeed = -vspeed;
		collide = true;
		show_debug_message("shiddy");
	}
	// Add random bounce direction
	var mvt = movements[movement];
	if (collide) {
		show_debug_message(str)
		if (irandom(100) <= mvt.randomizer) set_direction(direction + irandom_range(-90,90), mvt.numOfAngles);
	}
	// NOTE: "Security" isn't planned atm. (FIX/REWORK but it probably should be)
}

#endregion

#region CMovePath - Path Movement
function CMovePath_update() {
	if keyboard_check_pressed(vk_f2) then room_restart();
	var mvt = movements[movement];
	var n = mvt.nodes[mvt.node];
	if (n.reached) {
		if (mvt.reversing) mvt.node = max(mvt.node-1,0);
		if (!mvt.reverse && mvt.node != array_length(mvt.nodes)-1) {
			n.startX = -5552471;
			n.startY = -5552471;
			n.reached = false;
		}
		if (mvt.reverse && mvt.node == array_length(mvt.nodes)-1){
			mvt.reversing = true
			n.startX = -5552471;
			n.startY = -5552471;
			n.reached = false;
		}
		if (mvt.reverse && mvt.node <= array_length(mvt.nodes)-1 && !(mvt.reversing && mvt.node == 0)) {
			n.startX = -5552471;
			n.startY = -5552471;
			n.reached = false;
		}
		if (!mvt.reversing) mvt.node = min(mvt.node+1,array_length(mvt.nodes)-1);
		
	}
	n = mvt.nodes[mvt.node];
	
	
	
	if (n.startX == -5552471) {
		n.startX = x;
		n.startY = y;
	}
	if (n.pauseTimer > 0) {
		n.pauseTimer -= delta_time/1000;
	}
	else {
		var dir = 1;
		if (mvt.reversing) dir = -1; 
		var minX = min(n.startX, n.startX + (n.dx*dir));
		var maxX = max(n.startX, n.startX + (n.dx*dir));
		var minY = min(n.startY, n.startY + (n.dy*dir));
		var maxY = max(n.startY, n.startY + (n.dy*dir));
		
		if (mvt.posx != n.startX + n.dx)
			mvt.posx = clamp(mvt.posx + (dcos(point_direction(x,y,x + (n.dx*dir), y + (n.dy*dir))) * (get_pixels(n.spd) * dt())),minX,maxX);
		if (mvt.posy != n.startY + n.dy)
			mvt.posy = clamp(mvt.posy - (dsin(point_direction(x,y,x + (n.dx*dir), y + (n.dy*dir))) * (get_pixels(n.spd) * dt())),minY,maxY);
		hspeed = 0;
		vspeed = 0;
		direction = floor(n.dir * 11.25);
		if ((x == n.startX + n.dx && y == n.startY + n.dy) ||
			(x == n.startX + (n.dx*dir) && y == n.startY + (n.dy*dir) )) n.reached = true;
		else n.reached = false;
	}
	x = floor(mvt.posx);
	y = floor(mvt.posy);
	
}
#endregion

#region CMoveBullet - Bullet (launched object)

#endregion



#endregion