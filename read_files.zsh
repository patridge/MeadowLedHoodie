#!/bin/zsh

# Script to read files using meadow file read command
# This script loops through each filename and calls meadow file read {file}

## Folders
# Cache   [folder]
# crash   [folder]
# Data    [folder]
# Documents       [folder]
# sprites-soniccd [folder]
# sprites-sor2    [folder]
# sprites-tjearl  [folder]
# Temp    [folder]
# update-store    [folder]

# Array of filenames from your list
files=(
    "Apa102.dll"
    "Apa102.pdb"
    "Apa102.xml"
    "App.deps.json"
    "App.dll"
    "App.pdb"
    "As1115.xml"
    "Bh1750.dll"
    "Bh1750.pdb"
    "Bh1750.xml"
    "Bme68x.dll"
    "Bme68x.pdb"
    "Bme68x.xml"
    "Bmi270.dll"
    "Bmi270.pdb"
    "Bmi270.xml"
    "C16x9.xml"
    "defcon32-1.jpg"
    "defcon32-2.jpg"
    "defcon32-3.jpg"
    "dns.conf"
    "Is31fl3731.xml"
    "Mcp23xxx.dll"
    "Mcp23xxx.pdb"
    "Mcp23xxx.xml"
    "meadow.config.yaml"
    "Meadow.Contracts.dll"
    "Meadow.Contracts.pdb"
    "Meadow.Contracts.xml"
    "Meadow.deps.json"
    "Meadow.dll"
    "Meadow.F7.dll"
    "Meadow.F7.pdb"
    "Meadow.F7.xml"
    "Meadow.Foundation.dll"
    "Meadow.Foundation.pdb"
    "Meadow.Foundation.xml"
    "meadow.log"
    "Meadow.Logging.dll"
    "Meadow.Logging.pdb"
    "Meadow.Logging.xml"
    "Meadow.Modbus.xml"
    "Meadow.pdb"
    "Meadow.Units.dll"
    "Meadow.Units.pdb"
    "Meadow.Units.xml"
    "Meadow.xml"
    "MicroGraphics.dll"
    "MicroGraphics.pdb"
    "MicroGraphics.xml"
    "MicroJson.dll"
    "MicroJson.pdb"
    "MicroJson.xml"
    "Microsoft.Extensions.Configuration.Abstractions.dll"
    "Microsoft.Extensions.Configuration.dll"
    "Microsoft.Extensions.Configuration.FileExtensions.dll"
    "Microsoft.Extensions.FileProviders.Abstractions.dll"
    "Microsoft.Extensions.FileProviders.Physical.dll"
    "Microsoft.Extensions.FileSystemGlobbing.dll"
    "Microsoft.Extensions.Primitives.dll"
    "Mono.Security.dll"
    "MQTTnet.dll"
    "MQTTnet.pdb"
    "mscorlib.dll"
    "mscorlib.pdb"
    "NetEscapades.Configuration.Yaml.dll"
    "netstandard.dll"
    "ProjectLab.dll"
    "ProjectLab.pdb"
    "ProjectLab.xml"
    "Sc16is7x2.dll"
    "Sc16is7x2.pdb"
    "Sc16is7x2.xml"
    "SimpleJpegDecoder.dll"
    "System.Buffers.dll"
    "System.Configuration.dll"
    "System.Core.dll"
    "System.Core.pdb"
    "System.dll"
    "System.IO.Compression.dll"
    "System.IO.Compression.FileSystem.dll"
    "System.IO.Hashing.dll"
    "System.Memory.dll"
    "System.Net.Http.dll"
    "System.Numerics.dll"
    "System.pdb"
    "System.Security.dll"
    "System.Web.dll"
    "System.Xml.dll"
    "TextDisplayMenu.xml"
    "TftSpi.dll"
    "TftSpi.pdb"
    "TftSpi.xml"
    "Xpt2046.xml"
    "YamlDotNet.dll"
)

echo "Starting to read files using meadow file read command..."
echo "Total files to process: ${#files[@]}"
echo ""

# Loop through each file and call meadow file read
for file in "${files[@]}"; do
    echo "Reading file: $file"
    meadow file read "$file"
    
    # Add a small delay to avoid overwhelming the system (optional)
    # sleep 0.1
    
    echo "---"
done

echo ""
echo "Finished reading all files."
