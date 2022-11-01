modname = MartialArtist
gamepath = /mnt/c/Program\ Files\ \(x86\)/Steam/steamapps/common/Outward/Outward_Defed
pluginpath = BepInEx/plugins
sideloaderpath = $(pluginpath)/$(modname)/SideLoader

dependencies = CustomWeaponBehaviour SynchronizedWorldObjects TinyHelper

assemble:
	# common for all mods
	rm -f -r public
	mkdir -p public/$(pluginpath)/$(modname)
	cp bin/$(modname).dll public/$(pluginpath)/$(modname)/
	for dependency in $(dependencies) ; do \
		cp ../$${dependency}/bin/$${dependency}.dll public/$(pluginpath)/$(modname)/ ; \
	done
	
	# crusader specific
	mkdir -p public/$(sideloaderpath)/Items
	mkdir -p public/$(sideloaderpath)/Texture2D
	mkdir -p public/$(sideloaderpath)/AssetBundles
	
	mkdir -p public/$(sideloaderpath)/Items/Bastard/Textures
	cp resources/icons/bastard.png                          public/$(sideloaderpath)/Items/Bastard/Textures/icon.png
	cp resources/icons/bastard_small.png                    public/$(sideloaderpath)/Items/Bastard/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Block/Textures
	cp resources/icons/block.png                            public/$(sideloaderpath)/Items/Block/Textures/icon.png
	cp resources/icons/block_small.png                      public/$(sideloaderpath)/Items/Block/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Finesse/Textures
	cp resources/icons/finesse.png                          public/$(sideloaderpath)/Items/Finesse/Textures/icon.png
	cp resources/icons/finesse_small.png                    public/$(sideloaderpath)/Items/Finesse/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Parry/Textures
	cp resources/icons/parry.png                            public/$(sideloaderpath)/Items/Parry/Textures/icon.png
	cp resources/icons/parry_small.png                      public/$(sideloaderpath)/Items/Parry/Textures/skillicon.png
	
publish:
	make assemble
	rm -f $(modname).rar
	rar a $(modname).rar -ep1 public/*

install:
	make assemble
	rm -r -f $(gamepath)/$(pluginpath)/$(modname)
	cp -r public/* $(gamepath)
clean:
	rm -f -r public
	rm -f $(modname).rar
	rm -f -r bin
info:
	echo Modname: $(modname)
