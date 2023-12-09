modname = MartialArtist
gamepath = /mnt/c/Program\ Files\ \(x86\)/Steam/steamapps/common/Outward/Outward_Defed
pluginpath = BepInEx/plugins
sideloaderpath = $(pluginpath)/$(modname)/SideLoader

dependencies = CustomWeaponBehaviour SynchronizedWorldObjects TinyHelper IronCoin TinyQuests

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
	
	mkdir -p public/$(sideloaderpath)/Items/IronCoin/Textures
	cp -u resources/icons/IronCoin.png                         public/$(sideloaderpath)/Items/IronCoin/Textures/icon.png
	
	mkdir -p public/$(sideloaderpath)/Items/ThrowSand/Textures
	cp -u resources/icons/bastard.png                          public/$(sideloaderpath)/Items/ThrowSand/Textures/icon.png
	cp -u resources/icons/bastard_small.png                    public/$(sideloaderpath)/Items/ThrowSand/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/PrecisionStrike/Textures
	cp -u resources/icons/precision_strike.png                 public/$(sideloaderpath)/Items/PrecisionStrike/Textures/icon.png
	cp -u resources/icons/precision_strike_small.png           public/$(sideloaderpath)/Items/PrecisionStrike/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Forager/Textures
	cp -u resources/icons/forager.png                          public/$(sideloaderpath)/Items/Forager/Textures/icon.png
	cp -u resources/icons/forager_small.png                    public/$(sideloaderpath)/Items/Forager/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/CarefulMaintenance/Textures
	cp -u resources/icons/bastard.png                          public/$(sideloaderpath)/Items/CarefulMaintenance/Textures/icon.png
	cp -u resources/icons/bastard_small.png                    public/$(sideloaderpath)/Items/CarefulMaintenance/Textures/skillicon.png
	# mkdir -p public/$(sideloaderpath)/Items/HoneBlade/Textures
	# cp -u resources/icons/bastard.png                          public/$(sideloaderpath)/Items/HoneBlade/Textures/icon.png
	# cp -u resources/icons/bastard_small.png                    public/$(sideloaderpath)/Items/HoneBlade/Textures/skillicon.png
	
	mkdir -p public/$(sideloaderpath)/Items/Bastard/Textures
	cp -u resources/icons/bastard.png                          public/$(sideloaderpath)/Items/Bastard/Textures/icon.png
	cp -u resources/icons/bastard_small.png                    public/$(sideloaderpath)/Items/Bastard/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Reflexes/Textures
	cp -u resources/icons/reflexes.png                            public/$(sideloaderpath)/Items/Reflexes/Textures/icon.png
	cp -u resources/icons/reflexes_small.png                      public/$(sideloaderpath)/Items/Reflexes/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Finesse/Textures
	cp -u resources/icons/finesse.png                          public/$(sideloaderpath)/Items/Finesse/Textures/icon.png
	cp -u resources/icons/finesse_small.png                    public/$(sideloaderpath)/Items/Finesse/Textures/skillicon.png
	mkdir -p public/$(sideloaderpath)/Items/Parry/Textures
	cp -u resources/icons/parry.png                            public/$(sideloaderpath)/Items/Parry/Textures/icon.png
	cp -u resources/icons/parry_small.png                      public/$(sideloaderpath)/Items/Parry/Textures/skillicon.png
	
publish:
	make clean
	make assemble
	rar a $(modname).rar -ep1 public/*
	
	cp -r public/BepInEx thunderstore
	mv thunderstore/plugins/$(modname)/* thunderstore/plugins
	rmdir thunderstore/plugins/$(modname)
	
	(cd ../Descriptions && python3 $(modname).py)
	
	cp -u resources/manifest.json thunderstore/
	cp -u README.md thunderstore/
	cp -u resources/icon.png thunderstore/
	(cd thunderstore && zip -r $(modname)_thunderstore.zip *)
	cp -u ../tcli/thunderstore.toml thunderstore
	(cd thunderstore && tcli publish --file $(modname)_thunderstore.zip) || true
	mv thunderstore/$(modname)_thunderstore.zip .

install:
	make assemble
	rm -r -f $(gamepath)/$(pluginpath)/$(modname)
	cp -u -r public/* $(gamepath)
clean:
	rm -f -r public
	rm -f -r thunderstore
	rm -f $(modname).rar
	rm -f $(modname)_thunderstore.zip
	rm -f resources/manifest.json
	rm -f README.md
info:
	echo Modname: $(modname)
play:
	(make install && cd .. && make play)
edit:
	nvim ../Descriptions/$(modname).py
readme:
	(cd ../Descriptions/ && python3 $(modname).py)
