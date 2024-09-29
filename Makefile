include Makefile.helpers
modname = MartialArtist
# dependencies = CustomWeaponBehaviour 
dependencies = CustomWeaponBehaviour SynchronizedWorldObjects TinyHelper

assemble:
	# common for all mods
	rm -f -r public
	@make dllsinto TARGET=$(modname) --no-print-directory
	
	@make basefolders
	
	@make skill NAME="Bastard" FILENAME="bastard"
	@make skill NAME="Reflexes" FILENAME="reflexes"
	@make skill NAME="Finesse" FILENAME="finesse"
	@make skill NAME="Parry" FILENAME="parry"
	
forceinstall:
	make assemble
	rm -r -f $(gamepath)/$(pluginpath)/$(modname)
	cp -u -r public/* $(gamepath)
play:
	(make install && cd .. && make play)
