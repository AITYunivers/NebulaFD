namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public struct Event296Checks
    {
        public bool HasObjectInfo { get; set; }
        public bool HasEventFlags { get; set; }
        public bool HasParameters { get; set; }

        public static Event296Checks GetFromCond(short objType, short num)
        {
            Event296Checks ret = new();
			switch (objType)
			{
				case -7: // Player
					switch (num) 
					{
						case -6: // Joystick -> Repeat while joystick pressed
							break; // TODO
						case -5: // When number of lives reaches 0
							break; // TODO
						case -4: // Joystick -> Read joystick state
							break; // TODO
						case -3: // Compare to player's number of lives
							break; // TODO
						case -2: // Compare to player's score
							break; // TODO
					}
					break;
				case -6: // The mouse pointer and keyboard
					switch (num)
					{
						case -12: // The mouse -> When mouse wheel is moved down
							break; // TODO
						case -11: // The mouse -> When mouse wheel is moved down
							break; // TODO
						case -10: // The mouse -> Is mouse cursor displayed?
							break; // TODO
						case -9: // The keyboard -> Upon pressing any key
							break; // TODO
						case -8: // The mouse -> Repeat while mouse-key is pressed
							break; // TODO
						case -7: // The mouse -> User clicks on an object
							break; // TODO
						case -6: // The mouse -> User clicks within a zone
							break; // TODO
						case -5: // The mouse -> User clicks
							break; // TODO
						case -4: // The mouse -> Check for mouse pointer over an object
							break; // TODO
						case -3: // The mouse -> Check for mouse pointer in a zone
							break; // TODO
						case -2: // The keyboard -> Repeat while key is pressed
							break; // TODO
						case -1: // The keyboard -> Upon pressing a key
							break; // TODO
					}
					break;
				case -5: // New Objects
					switch (num)
					{
						case -23: // Pick all objects on a line
							break; // TODO
						case -22: // Pick objects with reference to their value -> Flags -> Off
							break; // TODO
						case -21: // Pick objects with reference to their value -> Flags -> On
							break; // TODO
						case -20: // Pick objects with reference to their value -> Alterable
							break; // TODO
						case -19: // Pick objects with reference to their value -> Fixed
							break; // TODO
						case -18: // Pick all objects in a zone
							break; // TODO
						case -17: // Pick an object at random
							break; // TODO
						case -16: // Pick a random object in a zone
							break; // TODO
						case -15: // Compare to total number of objects
							break; // TODO
						case -14: // Compare to number of objects in a zone
							break; // TODO
						case -13: // Test for no objects in a zone
							break; // TODO
					}
					break;
				case -4: // The Timer
					switch (num)
					{
						case -8: // Every
							break; // TODO
						case -7: // Is the timer equal to a certain value?
							break; // TODO
						case -6: // On event
							break; // TODO
						case -5: // User has left the computer for...
							break; // TODO
						case -4: // Every
							break; // TODO
						case -2: // Is the timer less than a certain value?
							break; // TODO
						case -1: // Is the timer greater than a certain value?
							break; // TODO
					}
					break;
				case -3: // Storyboard Controls
					switch (num)
					{
						case -10: // Frame position has just been saved
							break; // TODO
						case -9: // Frame position has just been loaded
							break; // TODO
						case -8: // End of pause
							break; // TODO
						case -7: // Is V-Sync Enabled?
							break; // TODO
						case -6: // Collision mask -> Is ladder
							break; // TODO
						case -5: // Collision mask -> Is obstacle
							break; // TODO
						case -4: // End of application
							break; // TODO
						case -2: // End of frame
							break; // TODO
						case -1: // Start of frame
							break; // TODO
					}
					break;
				case -2: // Sound
					switch (num)
					{
						// Music -> Is specific music not playing?
						// Music -> Has a music just finished?
						case -9: // Samples -> Is a specific channel paused?
							break; // TODO
						case -8: // Samples -> Is a specific channel not playing?
							break; // TODO
						case -7: // Music -> Is music paused?
							break; // TODO
						case -6: // Samples -> Is a specific sample paused?
							break; // TODO
						case -4: // Music -> Is music not playing?
							break; // TODO
						case -3: // Samples -> Is a sample not playing?
							break; // TODO
						case -1: // Samples -> Is a specific sample not playing?
							break; // TODO
						}
					break;
				case -1: // Special
					switch (num)
					{
						// Compare to global string
						// Application menu -> Is a menu option enabled?
						case -41: // Profiling -> Is profiling in progress
							break; // TODO
						case -40: // Runtime -> Is running as ...
							break; // TODO
						case -26: // X chances out of Y at random
							break; // TODO
						case -25: // OR (logical)
							break; // TODO
						case -24: // OR
							break; // TODO
						case -23: // Group of events -> On group activation
							break; // TODO
						case -22: // Is text available in clipboard
							break; // TODO
						case -21: // Application menu -> Close window has been selected
							break; // TODO
						case -20: // Application menu -> Is the menu bar visible?
							break; // TODO
						case -17: // Application menu -> Is a menu option checked?
							break; // TODO
						case -16: // On loop
							break; // TODO
						case -15: // Has files been dropped
							break; // TODO
						case -14: // Application menu -> Has an option been selected?
							break; // TODO
						case -12: // Group of events -> Check for activition
							break; // TODO
						case -8: // Compare to a global value
							break; // TODO
						case -7: // Limit conditions -> Only one action when event loops
							break; // TODO
						case -6: // Limit conditions -> Run this event once
							break; // TODO
						case -5: // Limit conditions -> Repeat
							break; // TODO
						case -4: // Limit conditions -> Restrict actions
							break; // TODO
						case -3: // Compare two general values
							break; // TODO
						case -2: // Never
							break; // TODO
						case -1: // Always
							break;
					}
					break;
				case >= 0:
					switch (num)
					{
						default:
							if (objType < 32) // Object Type specific conditions
								switch (num)
								{
									case -84:
										if (objType == 9) // (Sub-Application) Is application paused?
											break; // TODO
										else // Compare Y scale to a value
											break; // TODO
									case -83:
										if (objType == 4) // (Question and Answer) Is the answer equal to a certain value?
											break; // TODO
										else if (objType == 9) // (Sub-Application) Is application visible?
											break; // TODO
										else
											break; // TODO
									case -82:
										if (objType == 4) // (Question and Answer) Is the answer false?
											break; // TODO
										else if (objType == 9) // (Sub-Application) Is application finished?
											break; // TODO
										else // Compare angle to a value
											break; // TODO
									case -81:
										if (objType == 4) // (Question and Answer) Is the answer correct?
											break; // TODO
										else if (objType == 9) // (Sub-Application) Frame has changed?
											break; // TODO
										else // GetObjectName GetComparison Parameters (TBD)
											break; // TODO
								}
							else // Extension
							{

							}
							break;
						case -51: // Collisions -> Would "Object Name" overlap a backdrop at ...?
							break; // TODO
						case -50: // Collisions -> Would "Object Name" overlap another object at ...?
							break; // TODO
						case -49: // Compare to instance value
							break; // TODO
						case -48: // Pick or count -> Pick "Object Name" objects with a maximum expression value
							break; // TODO
						case -47: // Pick or count -> Pick "Object Name" objects with a minimum expression value
							break; // TODO
						case -46: // Position -> Compare layer to a value
							break; // TODO
						case -45: // Compare expression to a value
							break; // TODO
						case -44: // Pick or count -> Pick closest "Object Name" object from ...
							break; // TODO
						case -41: // Loops -> On each object
							break; // TODO
						case -40: // Text -> Is font strikeout?
							break; // TODO
						case -39: // Text -> Is font underlined?
							break; // TODO
						case -38: // Text -> Is font italic?
							break; // TODO
						case -37: // Text -> Is font bold?
							break; // TODO
						case -36: // Compare to one of the alterable strings
							break; // TODO
						case -35: // Movement -> Path movement -> Path movement of "Object Name" has reached a named node
							break; // TODO
						case -34: // Pick or count -> Pick "Object Name" at random
							break; // TODO
						case -33: // Pick or count -> Have all "Object Names" been destroyed
							break; // TODO
						case -32: // Pick or count -> Compare to number of "Object Name" objects
							break; // TODO
						case -31: // Pick or count -> Text for no "Object Name" objects in a zone
							break; // TODO
						case -30: // Pick or count -> Compare to the number of "Object Name" objects in a zone
							break; // TODO
						case -29: // Visibility -> Is "Object Name" visible
							break; // TODO
						case -28: // Visibility -> Is "Object Name" invisible
							break; // TODO
						case -27: // Compare to one of the alterable values
							break; // TODO
						case -26: // Compare to fixed value
							break; // TODO
						case -25: // Flags -> Is a flag on?
							break; // TODO
						case -24: // Flags -> Is a flag off?
							break; // TODO
						case -23: // Collisions -> Overlapping a backdrop
							break; // TODO
						case -22: // Position -> Is "Object Name" getting close to window's edge
							break; // TODO
						case -21: // Movement -> Path movement -> Has "Object Name" reached the end in its path
							break; // TODO
						case -20: // Movement -> Path movement -> Has "Object Name" reacehd a node of the path
							break; // TODO
						case -19: // Movement -> Compare Acceleration of "Object Name" to a value
							break; // TODO
						case -18: // Movement -> Compare Deceleration of "Object Name" to a value
							break; // TODO
						case -17: // Position -> Compare X position to a value
							break; // TODO
						case -16: // Position -> Compare Y position to a value
							break; // TODO
						case -15: // Movement -> Compare Speed of "Object Name" to a value
							ret.HasObjectInfo = true;
							ret.HasEventFlags = true;
							ret.HasParameters = true;
							break;
						case -14: // Collisions -> Another object
							break; // TODO
						case -13: // Collisions -> Backdrop
							break; // TODO
						case -12: // Position -> Test position of "Object Name" -> Is object leaving the frame
							break; // TODO
						case -11: // Position -> Test position of "Object Name" -> Is object entering the frame
							break; // TODO
						case -10: // Position -> Test position of "Object Name" -> Is object out of the frame
							break; // TODO
						case -9: // Position -> Test position of "Object Name" -> Is object in the frame
							break; // TODO
						case -8: // Direction -> Compare direction of "Object Name"
							break; // TODO
						case -7: // Movement -> Is "Object Name" stopped?
							break; // TODO
						case -6: // Movement -> Is "Object Name" bouncing?
							break; // TODO
						case -4: // Collisions -> Overlapping another object
							break; // TODO
						case -3: // Animation -> Which animation of "Object Name" is playing?
							break; // TODO
						case -2: // Animation -> Has an animation finished?
							break; // TODO
						case -1: // Animation -> Compare current frame of "Object Name" to a value
							break; // TODO
					}
					break;
			}
			return ret;
		}
		public static Event296Checks GetFromAct(short objType, short num)
		{
			Event296Checks ret = new();
			switch (objType)
			{
				case -7: // Player
					switch (num)
					{
						case 0: // Score -> Set Score
							break; // TODO
						case 1: // Number of Lives -> Set Number of Lives
							break; // TODO
						case 2: // Player Control -> Ignore Control
							break; // TODO
						case 3: // Player Control -> Restore Control
							break; // TODO
						case 4: // Score -> Add to Score
							break; // TODO
						case 5: // Number of Lives -> Add to Number of Lives
							break; // TODO
						case 6: // Score -> Subtract from Score
							break; // TODO
						case 7: // Number of Lives -> Subtract from Number of Lives
							break; // TODO
						case 8: // Player Control -> Set input device
							break; // TODO
						case 9: // Player Control -> Set key
							break; // TODO
						case 10: // Set player name
							break; // TODO
					}
					break;
				case -6: // The mouse pointer and keyboard
					switch (num)
					{
						case 0: // Hide Windows mouse pointer
							break; // TODO
						case 1: // Show Windows mouse pointer
							break; // TODO
						case 2: // Reset inputs between frames ...
							break; // TODO
					}
					break;
				case -5: // New Objects
					switch (num)
					{
						case 0: // Create object
							break; // TODO
						case 1: // Create object at ...
							break; // TODO
						case 2: // Create object by name
							break; // TODO
						case 3: // Create object by name at ...
							break; // TODO
					}
					break;
				case -4: // The Timer
					switch (num)
					{
						case 0: // Set timer
							break; // TODO
						case 1: // Fire event after given delay
							break; // TODO
						case 2: // Fire event N times after given delay
							break; // TODO
					}
					break;
				case -3: // Storyboard Controls
					switch (num)
					{
						case 0: // Next frame
							break;
						case 1: // Previous frame
							break; // TODO
						case 2: // Jump to frame
							break; // TODO
						case 4: // End the application
							break; // TODO
						case 5: // Restart the application
							break; // TODO
						case 6: // Restart the current frame
							break; // TODO
						case 7: // Scrollings -> Center window position in frame
							break; // TODO
						case 8: // Scrollings -> Center horizontal position of window in frame
							break; // TODO
						case 9: // Scrollings -> Center vertical position of window in frame
							break; // TODO
						case 12: // Screen -> Clear screen
							break; // TODO
						case 13: // Screen -> Clear zone
							break; // TODO
						case 14: // Screen -> Full Screen Mode
							break; // TODO
						case 15: // Screen -> Windowed Mode
							break; // TODO
						case 16: // Set frame rate
							break; // TODO
						case 17: // Pause application -> Pause and resume when a key is pressed
							break; // TODO
						case 18: // Pause application -> Pause and resume when any key is pressed
							break; // TODO
						case 19: // V-Sync -> On
							break; // TODO
						case 20: // V-Sync -> Off
							break; // TODO
						case 21: // Frame -> Set Virtual Width
							break; // TODO
						case 22: // Frame -> Set Virtual Height
							break; // TODO
						case 23: // Frame -> Set Background Color
							break; // TODO
						case 24: // Backdrops -> Delete Created Backdrops At
							break; // TODO
						case 25: // Backdrops -> Delete All Created Backdrops
							break; // TODO
						case 26: // Frame -> Set Frame Width
							break; // TODO
						case 27: // Frame -> Set Frame Height
							break; // TODO
						case 28: // Frame Position -> Save frame position
							break; // TODO
						case 29: // Frame Position -> Load frame position
							break; // TODO
						case 30: // Frame Position -> Load application position
							break; // TODO
						case 31: // Demo -> Play demo file
							break; // TODO
						case 32: // Frame -> Effect -> Set effect
							break; // TODO
						case 33: // Frame -> Effect -> Set effect parameter
							break; // TODO
						case 34: // Frame -> Effect -> Set effect image parameter
							break; // TODO
						case 35: // Frame -> Effect -> Set alpha-blending coefficient
							break; // TODO
						case 36: // Frame -> Effect -> Set RGB coefficient
							break; // TODO
						case 37: // Set anti-aliasing when resizing
							break; // TODO
					}
					break;
				case -2: // Sound
					switch (num)
					{
						// Music -> Play Music
						// Music -> Play and Loop Music
						case 0: // Samples -> Play Sample
							break; // TODO
						case 1: // Samples -> Stop any Sample Playing
							break; // TODO
						case 3: // Music -> Stop any Music Playing
							break; // TODO
						case 4: // Samples -> Play and Loop Sample
							break; // TODO
						case 6: // Samples -> Stop a Specific Sample
							break; // TODO
						case 7: // Samples -> Pause sample
							break; // TODO
						case 8: // Samples -> Resume sample
							break; // TODO
						case 9: // Music -> Pause music
							break; // TODO
						case 10: // Music -> Resume music
							break; // TODO
						case 11: // Samples -> Play Sample on a specific channel
							break; // TODO
						case 12: // Samples -> Play and Loop Sample on a specific channel
							break; // TODO
						case 13: // Samples -> Pause Channel
							break; // TODO
						case 14: // Samples -> Resume Channel
							break; // TODO
						case 15: // Samples -> Stop Channel
							break; // TODO
						case 16: // Samples -> Set Channel Position
							break; // TODO
						case 17: // Samples -> Set Channel Volume
							break; // TODO
						case 18: // Samples -> Set Channel Pan
							break; // TODO
						case 19: // Samples -> Set Sample Position
							break; // TODO
						case 20: // Samples -> Set Main Volume
							break; // TODO
						case 21: // Samples -> Set Sample Volume
							break; // TODO
						case 22: // Samples -> Set Main Pan
							break; // TODO
						case 23: // Samples -> Set Sample Pan
							break; // TODO
						case 24: // Samples -> Pause All Sounds
							break; // TODO
						case 25: // Samples -> Resume All Sounds
							break; // TODO
						case 26: // Music -> Play Music File
							break; // TODO
						case 27: // Music -> Play and Loop Music File
							break; // TODO
						case 28: // Samples -> Play Sample File on a specific channel
							break; // TODO
						case 29: // Samples -> Play and Loop Sample File on a specific channel
							break; // TODO
						case 30: // Samples -> Lock Channel
							break; // TODO
						case 31: // Samples -> Unlock Channel
							break; // TODO
						case 32: // Samples -> Set Channel Frequency
							break; // TODO
						case 33: // Samples -> Set Sample Frequency
							break; // TODO
						case 34: // Samples -> Preload sample file
							break; // TODO
						case 35: // Samples -> Discord sample file
							break; // TODO
						case 36: // Samples -> Play Samples (all parameters)
							break; // TODO
					}
					break;
				case -1: // Special
					switch (num)
					{
						case 2: // Execute an external program -> With a fixed pathname
							break; // TODO
						case 3: // Change a global value -> Set
							break; // TODO
						case 4: // Change a global value -> Subtract from
							break; // TODO
						case 5: // Change a global value -> Add to
							break; // TODO
						case 6: // Group of events -> Activate
							break; // TODO
						case 7: // Group of events -> Deactivate
							break; // TODO
						case 8: // Application menu -> Enable
							break; // TODO
						case 9: // Application menu -> Disable
							break; // TODO
						case 10: // Application menu -> Check
							break; // TODO
						case 11: // Application menu -> Uncheck
							break; // TODO
						case 12: // Application menu -> Show menu bar
							break; // TODO
						case 13: // Application menu -> Hide menu bar
							break; // TODO
						case 14: // Fast loops -> Start loop
							break; // TODO
						case 15: // Fast loops -> Stop loop
							break; // TODO
						case 16: // Fast loops -> Set loop index
							break; // TODO
						case 17: // Randomize
							break; // TODO
						case 18: // Application menu -> Send Menu Command
							break; // TODO
						case 19: // Set global string
							break; // TODO
						case 20: // Clipboard -> Send text to clipboard
							break; // TODO
						case 21: // Clipboard -> Clear clipboard
							break; // TODO
						case 22: // Execute an external program -> With an evaluated pathname
							break; // TODO
						case 23: // Debugger -> Open debugger
							break; // TODO
						case 24: // Debugger -> Pause debugger
							break; // TODO
						case 25: // Binary Files -> Extract binary file
							break; // TODO
						case 26: // Binary Files -> Release binary file
							break; // TODO
						case 39: // Profiling -> Start profiling
							break; // TODO
						case 40: // Profiling->Stop profiling
							break; // TODO
						case 41: // Debugger -> Clear output window
							break; // TODO
						case 42: // Debugger -> Send text to output window
							break; // TODO
						case 44: // Child events -> Break
							break; // TODO
					}
					break;
				case >= 0:
					switch (num)
					{
						default:
							if (objType < 32) // Object Type specific conditions
								switch (num)
								{
									case 80:
										if (objType == 3) // (String) Erase Text...
											break; // TODO
										else if (objType == 4) // (Question and Answer) Ask the question
											break; // TODO
										else if (objType == 7) // (Counter) Set Counter
											break; // TODO
										else if (objType == 8) // (Formatted Text) Set horizontal position of text
											break; // TODO
										else if (objType == 9) // (Sub-Application) Restart Application
											break; // TODO
										else // (Active) Animation -> Paste image into background
											break; // TODO
									case 81:
										if (objType == 2) // (Active) Bring to front
											break; // TODO
										else if (objType == 7) // (Counter) Add to Counter
											break; // TODO
										else if (objType == 8) // (Formatted Text) Set vertical position of text
											break; // TODO
										else if (objType == 9) // (Sub-Application) Restart Current Frame
											break; // TODO
										else // (String) Display Text...
											break; // TODO
									case 82:
										if (objType == 7) // (Counter) Subtract from Counter
											break; // TODO
										else if (objType == 8) // (Formatted Text) Set zoom factor
											break; // TODO
										else if (objType == 9) // (Sub-Application) Next Frame
											break; // TODO
										else // (String) Flash Text...
											break; // TODO
									case 83:
										if (objType == 3) // (String) Set Color of Text...
											break; // TODO
										else if (objType == 7) // (Counter) Set minimum value
											break; // TODO
										else if (objType == 8) // (Formatted Text) Select -> Clear
											break; // TODO
										else if (objType == 9) // (Sub-Application) Previous Frame
											break; // TODO
										else // (Active) Animation -> Add backdrop
											break; // TODO
									case 84:
										if (objType == 3) // (String) Set paragraph...
											break; // TODO
										else if (objType == 7) // (Counter) Set maximum value
											break; // TODO
										else if (objType == 8) // (Formatted Text) Select -> From Search -> One word
											break; // TODO
										else if (objType == 9) // (Sub-Application) End Application
											break; // TODO
										else // (Active) Animation -> Replace Color
											break; // TODO
									case 85:
										if (objType == 3) // (String) Previous paragraph
											break; // TODO
										else if (objType == 7) // (Counter) Bars -> Set Color
											break; // TODO
										else if (objType == 8) // (Formatted Text) Select -> From Search -> Next word
											break; // TODO
										else if (objType == 9) // (Sub-Application) Choose new Application
											break; // TODO
										else // (Active) Scale / Angle -> Set Scale
											break; // TODO
									case 86:
										if (objType == 3) // (String) Next paragraph
											break; // TODO
										else if (objType == 7) // (Counter) Bars -> Set Color #2
											break; // TODO
										else if (objType == 8) // (Formatted Text) Select -> From Search -> All words
											break; // TODO
										else if (objType == 9) // (Sub-Application) Jump to a Frame...
											break; // TODO
										else // (Active) Scale / Angle -> Set X Scale
											break; // TODO
									case 87:
										if (objType == 3) // (String) 
											break; // TODO
										else if (objType == 8) // (Formatted Text) 
											break; // TODO
										else if (objType == 9) // (Sub-Application) 
											break; // TODO
										else // (Active) Scale / Angle -> Set Y Scale
											break; // TODO
									case 88:
										if (objType == 3) // (String) Change alterable string
											break; // TODO
										else if (objType == 8) // (Formatted Text) Select -> Line Num
											break; // TODO
										else // (Active) Scale / Angle -> Set Angle
											break; // TODO
									case 89:
										if (objType == 8) // (Formatted Text) Select -> Paragraph Num
											break; // TODO
										else // (Active) Animation -> Load Frame
											break; // TODO
									case 90:
										if (objType == 9) // (Sub-Application) 
											break; // TODO
										else // (Active) Animation -> Preload Image
											break; // TODO
									case 91:
										if (objType == 8) // (Formatted Text) Select -> All text
											break; // TODO
										else if (objType == 9) // (Sub-Application) Pause Application
											break; // TODO
										else // (Active) Animation -> Load Animations
											break; // TODO
									case 92:
										if (objType == 9) // (Sub-Application) Resume Application
											break; // TODO
										else // (Formatted Text) Select -> Word Num
											break; // TODO
									case 93: // (Sub-Application) Size -> Set width
										break; // TODO
									case 94:
										if (objType == 9) // (Sub-Application) Size -> Set height
											break; // TODO
										else // (Formatted Text) Focus -> Set to word number
											break; // TODO
									case 95: // (Formatted Text) Word -> Off
										break; // TODO
									case 96: // (Formatted Text) Word -> Text -> Color
										break; // TODO
									case 97: // (Formatted Text) Word -> Text -> Bold
										break; // TODO
									case 98: // (Formatted Text) Word -> Text -> Italic
										break; // TODO
									case 99: // (Formatted Text) Word -> Text -> Underlined
										break; // TODO
									case 100: // (Formatted Text) Word -> Text -> Outline
										break; // TODO
									case 101: // (Formatted Text) Word -> Background -> Color
										break; // TODO
									case 103: // (Formatted Text) Word -> Background -> Marker
										break; // TODO
									case 104: // (Formatted Text) Word -> Background -> Hatched
										break; // TODO
									case 105: // (Formatted Text) Word -> Background -> Inverted
										break; // TODO
									case 106: // (Formatted Text) Display
										break; // TODO
									case 107: // (Formatted Text) Focus -> Set to previous word
										break; // TODO
									case 108: // (Formatted Text) Focus -> Set to next word
										break; // TODO
									case 109: // (Formatted Text) Focus -> Remove
										break; // TODO
									case 112: // (Formatted Text) Load -> Load new text
										break; // TODO
									case 113: // (Formatted Text) Load -> Insert new text
										break; // TODO
									case 114: // (Formatted Text) Load -> Insert string
										break; // TODO
								}
							else // Extension
							{

							}
							break;
						case 0: // Flags -> Set
							break; // TODO
						case 1: // Position -> Select Position...
							break; // TODO
						case 2: // Position -> Set X Coordinate...
							break; // TODO
						case 3: // Position -> Set Y Coordinate...
							break; // TODO
						case 4: // Movement -> Stop
							break; // TODO
						case 5: // Movement -> Start
							break; // TODO
						case 6: // Movement -> Set Speed...
							break; // TODO
						case 7: // Movement -> Set Maximum Speed...
							break; // TODO
						case 8: // Movement -> Wrap Around Play Area
							break; // TODO
						case 9: // Movement -> Bounce
							break; // TODO
						case 10: // Movement -> Reverse
							break; // TODO
						case 11: // Movement -> Multiple movements -> Next movement
							break; // TODO
						case 12: // Movement -> Multiple movements -> Previous movement
							break; // TODO
						case 13: // Movement -> Multiple movements -> Select movement
							break; // TODO
						case 14: // Direction -> Look in the direction of ...
							break; // TODO
						case 15: // Animation -> Stop
							break; // TODO
						case 16: // Animation -> Start
							break; // TODO
						case 17: // Animation -> Change -> Animation sequence...
							break; // TODO
						case 18: // Animation -> Change -> Direction of animation...
							break; // TODO
						case 19: // Animation -> Change -> Speed of animation...
							break; // TODO
						case 20: // Animation -> Restore -> Animation sequence...
							break; // TODO
						case 21: // Animation -> Restore -> Direction of animation...
							break; // TODO
						case 22: // Animation -> Restore -> Speed of animation...
							break; // TODO
						case 23: // Direction -> Select Direction...
							break; // TODO
						case 24: // Destroy
							break; // TODO
						case 25: // Position -> Swap Position with Another Object
							break; // TODO
						case 26: // Visibility -> Make Object Invisible
							break; // TODO
						case 27: // Visibility -> Make Object Reappear
							break; // TODO
						case 28: // Visibility -> Flash Object
							break; // TODO
						case 29: // Launch an Object...
							break; // TODO
						case 30: // Launch an Object... -> Launch in direction of...
							break; // TODO
						case 31: // Alterable Values -> Set
							break; // TODO
						case 32: // Alterable Values -> Add to
							break; // TODO
						case 33: // Alterable Values -> Subtract from
							break; // TODO
						case 34: // Alterable Values -> Spread value
							break; // TODO
						case 35: // Flags -> Set On
							break; // TODO
						case 36: // Flags -> Set Off
							break; // TODO
						case 37: // Flags -> Toggle
							break; // TODO
						case 38: // Effect -> Compatibility -> Change ink effect
							break; // TODO
						case 39: // Effect -> Compatibility -> Set semi-transparency
							break; // TODO
						case 40: // Animation -> Change -> Animation frame...
							break; // TODO
						case 41: // Animation -> Restore -> Animation frame
							break; // TODO
						case 42: // Movement -> Set acceleration...
							break; // TODO
						case 43: // Movement -> Set deceleration...
							break; // TODO
						case 44: // Movement -> Set rotating speed...
							break; // TODO
						case 45: // Movement -> Set authorised directions...
							break; // TODO
						case 46: // Movement -> Path movement -> Branch node...
							break; // TODO
						case 47: // Movement -> Set gravity...
							break; // TODO
						case 48: // Movement -> Path movement -> Goto node...
							break; // TODO
						case 49: // Alterable Strings -> Set
							break; // TODO
						case 50: // Text -> Set font name
							break; // TODO
						case 51: // Text -> Set font size
							break; // TODO
						case 52: // Text -> Set bold
							break; // TODO
						case 53: // Text -> Set italic
							break; // TODO
						case 54: // Text -> Set underline
							break; // TODO
						case 55: // Text -> Set strikeout
							break; // TODO
						case 56: // Text -> Set text color
							break; // TODO
						case 57: // Order -> Bring to front
							break; // TODO
						case 58: // Order -> Bring to back
							break; // TODO
						case 59: // Order -> Move behind object
							break; // TODO
						case 60: // Order -> Move in front of object
							break; // TODO
						case 61: // Order -> Move to layer
							break; // TODO
						case 62: // Debugger -> Add object to debugger
							break; // TODO
						case 63: // Effect -> Set effect
							break; // TODO
						case 64: // Effect -> Set effect parameter
							break; // TODO
						case 65: // Effect -> Set alpha-blending coefficient
							break; // TODO
						case 66: // Effect -> Set RGB coefficient
							break; // TODO
						case 67: // Effect -> Set effect image parameter
							break; // TODO
						case 68: // Movement -> Physics -> Set friction...
							break; // TODO
						case 69: // Movement -> Physics -> Set elasticity...
							break; // TODO
						case 70: // Movement -> Physics -> Apply impulse...
							break; // TODO
						case 71: // Movement -> Physics -> Apply angular impulse...
							break; // TODO
						case 72: // Movement -> Physics -> Apply force...
							break; // TODO
						case 73: // Movement -> Physics -> Apply torque...
							break; // TODO
						case 74: // Movement -> Physics -> Set linear velocity...
							break; // TODO
						case 75: // Movement -> Physics -> Set angular velocity...
							break; // TODO
						case 76: // Count -> For each object
							break; // TODO
						case 77: // Count -> For each of two objects
							break; // TODO
						case 78: // Movement -> Physics -> Stop force
							break; // TODO
						case 79: // Movement -> Physics -> Stop torque
							break; // TODO
					}
					break;
			}
			return ret;
        }
    }
}
