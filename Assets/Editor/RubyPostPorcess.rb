require 'yaml'
require 'pathname'
require 'ostruct'
require 'fileutils'
require 'erb'

workingPath=ARGV[0]
buildPath=ARGV[1]

soPath="#{workingPath}/Assets/Assets/ScriptableObjects"
picturePath = "#{workingPath}/Assets/Assets/Textures/Pictures"
allAlbumsMeta=  "#{soPath}/AllAlbums.asset"
enLocaleFile = "#{workingPath}/Assets/Assets/Localisation/English.asset"
ruLocaleFile = "#{workingPath}/Assets/Assets/Localisation/Russian.asset"
LANGUAGE=1

pictureCache = {}


def getLocaleObject(localeFile)
  result = {}
  localeFileObject = YAML.load_file(localeFile)
  localeFileObject["MonoBehaviour"]["entries"].each do |entry|
     result[entry["key"]] = entry["value"]
  end
  return result
end

english = getLocaleObject(enLocaleFile)
russian = getLocaleObject(ruLocaleFile)

Dir["#{picturePath}/**/Resources/**/*.meta"].each do |metaFile|
  metaObj = YAML.load_file("#{metaFile}")
  if metaObj["TextureImporter"] != nil
    filePath = metaFile.sub(".meta","")
    pictureCache[File.basename(filePath,".*")]=filePath
  end
end

allAssets={}
Dir["#{soPath}/**/*.asset"].each do |assetPath|
  assetObj = YAML.load_file("#{assetPath}")
  id = YAML.load_file("#{assetPath}.meta")["guid"]
  allAssets[id]=assetObj
end

Album = Struct.new(:name, :caption, :ruCaption, :enCaption, :pictures, :relativePath)
Picture = Struct.new(:name, :caption, :ruCaption, :enCaption, :picturePath, :iconPath, :iconRelativePath, :pictureRelativePath)

allAlbums= YAML.load_file("#{allAlbumsMeta}")
albums = []
allAlbums["MonoBehaviour"]["album"].each do |albumMeta|
  if  albumMeta["sheetList"] != nil
    albumId = albumMeta["sheetList"]["guid"]
    if albumId != nil
      albumObj = allAssets[albumId]

      name = albumObj["MonoBehaviour"]["m_Name"]
      nameKey = albumObj["MonoBehaviour"]["nameKey"]
      ruCaption = russian[nameKey]
      enCaption = english[nameKey]
      album = Album.new(name, ruCaption, ruCaption, enCaption);
      pictures = []
      albumObj["MonoBehaviour"]["sheetList"].each do |sheetList|
        sheetObject = allAssets[sheetList["guid"]]
        if sheetObject != nil
          picture = Picture.new()
          picture.name = sheetObject["MonoBehaviour"]["m_Name"]
          picture.ruCaption = russian[sheetObject["MonoBehaviour"]["nameKey"]]
          picture.enCaption = english[sheetObject["MonoBehaviour"]["nameKey"]]
          picture.caption = picture.ruCaption
          picture.picturePath = pictureCache[ sheetObject["MonoBehaviour"]["persistentFrontOutlinePath"]]
          picture.iconPath = picture.picturePath.sub(".outline.psd",".icon.png")
          pictures.push(picture)
        end
      end
      album.pictures = pictures
      albums.push(album)
    end
  end
end

def createDirectoryIfNotExists(directoryName)
  Dir.mkdir(directoryName) unless File.exists?(directoryName)
end


albumRootRelativeDir="img/albums"
albumRootDir="#{buildPath}/#{albumRootRelativeDir}"
createDirectoryIfNotExists albumRootDir
albums.each do |album|
  albumRelativeDir="#{albumRootRelativeDir}/#{album["name"].downcase}"
  album["relativePath"]=albumRelativeDir
  albumDir="#{albumRootDir}/#{album["name"].downcase}"
  createDirectoryIfNotExists(albumDir)
  album.pictures.each do |picture|

    iconRelativePath="#{albumRelativeDir}/#{picture["name"].downcase}.icon.png"
    picture["iconRelativePath"]=iconRelativePath
    pictureRelativePath="#{albumRelativeDir}/#{picture["name"].downcase}.png"
    picture["pictureRelativePath"]=pictureRelativePath
    pictureIconFullPath="#{buildPath}/#{iconRelativePath}"
    FileUtils.cp(picture["iconPath"],pictureIconFullPath)
    pictureFullPath="#{buildPath}/#{pictureRelativePath}"
    FileUtils.cp(picture["picturePath"],pictureFullPath)
    cmd1=<<-eos
      /opt/local/bin/convert #{pictureIconFullPath} -flatten #{pictureIconFullPath}
    eos

    cmd2=<<-eos
      /opt/local/bin/convert #{pictureFullPath}    -resize 1024x768^ -gravity center -extent 1027x724  #{pictureFullPath}
    eos

    cmd3=<<-eos
      /opt/local/bin/convert #{pictureFullPath}  -flatten -pointsize 20 -fill white  -undercolor '#00000080'  -gravity SouthWest -annotate +5+5 'http://colorus.info' #{pictureFullPath}
    eos
    `#{cmd1}`
    `#{cmd2}`
    `#{cmd3}`
  end
end



class Page
  def initialize title, albumsInfo, erbDir
    @pageTitle = title
    @erbDir = erbDir
    @albums = albumsInfo
  end

  def render path
    content = File.read(File.expand_path(@erbDir+"/"+path))
    t = ERB.new(content)
    t.result(binding)
  end
end


erbDir="#{workingPath}/Assets/WebPlayertemplates/colorus.info/erb"
page = Page.new("Colorus - детская раскраска", albums, erbDir)

result=page.render("index.html.erb")

f = File.open("#{buildPath}/index.html", 'w')
f.puts result
f.close

result=page.render("gallery.html.erb")
f = File.open("#{buildPath}/gallery.html", 'w')
f.puts result
f.close
