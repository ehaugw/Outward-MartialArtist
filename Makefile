modname = MartialArtist
gamepath = /mnt/c/Program\ Files\ \(x86\)/Steam/steamapps/common/Outward/Outward_Defed
pluginpath = BepInEx/plugins
sideloaderpath = $(pluginpath)/$(modname)/SideLoader

dependencies = CustomWeaponBehaviour SynchronizedWorldObjects TinyHelper

assemble:
	# common for all mods
	rm -f -r public
	mkdir -p public/$(pluginpath)/$(modname)
	cp -u bin/$(modname).dll public/$(pluginpath)/$(modname)/
	for dependency in $(dependencies) ; do \
		cp -u ../$${dependency}/bin/$${dependency}.dll public/$(pluginpath)/$(modname)/ ; \
	done
	
	# mod specific
	mkdir -p public/$(sideloaderpath)/Items
	mkdir -p public/$(sideloaderpath)/Texture2D
	mkdir -p public/$(sideloaderpath)/AssetBundles
	
	mkdir -p public/$(sideloaderpath)/Items/Bastard/Textures
	cp -u resources/icons/bastard.png                          public/$(sideloaderpath)/Items/Bastard/Textures/icon.png
	cp -u resources/icons/bastard_small.png                    public/$(sideloaderpath)/Items/Bastard/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Block/Textures
	cp -u resources/icons/block.png                            public/$(sideloaderpath)/Items/Block/Textures/icon.png
	cp -u resources/icons/block_small.png                      public/$(sideloaderpath)/Items/Block/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Finesse/Textures
	cp -u resources/icons/finesse.png                          public/$(sideloaderpath)/Items/Finesse/Textures/icon.png
	cp -u resources/icons/finesse_small.png                    public/$(sideloaderpath)/Items/Finesse/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Parry/Textures
	cp -u resources/icons/parry.png                            public/$(sideloaderpath)/Items/Parry/Textures/icon.png
	cp -u resources/icons/parry_small.png                      public/$(sideloaderpath)/Items/Parry/Textures/skillicon.png
	
publish:
	make clean
	make assemble
	rm -f $(modname).rar
	rar a $(modname).rar -ep1 public/*
	
	(cd ../Descriptions && python3 $(modname).py)
	
	cp -u resources/manifest.json public/BepInEx/
	cp -u README.md public/BepInEx/
	cp -u resources/icon.png public/BepInEx/
	(cd public/BepInEx && zip -r $(modname)_thunderstore.zip * && mv $(modname)_thunderstore.zip ../../)

install:
	make assemble
	rm -r -f $(gamepath)/$(pluginpath)/$(modname)
	cp -u -r public/* $(gamepath)
clean:
	rm -f -r public
	rm -f $(modname).rar
	rm -f $(modname)_thunderstore.zip
info:
	echo Modname: $(modname)
