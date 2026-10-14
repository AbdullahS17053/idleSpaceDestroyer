$path = "D:\Gaming\IdleSpaceDestroyer\idleSpaceDestroyer\IdleSpace\Assets\Scenes\GameScene.unity"
$content = [System.IO.File]::ReadAllText($path)

# 1. Update sprite of Planet (5) (Image on GO 1649789104) to Planet_Candy (7424acc3e842a4d49845700a2fd1a10e)
$content = $content -replace "(--- !u!114 &\d+[\s\S]*?m_GameObject: \{fileID: 1649789104\}[\s\S]*?m_Sprite: \{fileID: 21300000, guid: )[a-f0-9]{32}", "`${1}7424acc3e842a4d49845700a2fd1a10e"

# 2. Update sprite of Planet (17) (Image on GO 929201232) to Planet_Sun2 (beb60817b2e44914795c796169770540)
$content = $content -replace "(--- !u!114 &\d+[\s\S]*?m_GameObject: \{fileID: 929201232\}[\s\S]*?m_Sprite: \{fileID: 21300000, guid: )[a-f0-9]{32}", "`${1}beb60817b2e44914795c796169770540"

# 3. Swap RT 929201233 and RT 1649789105 in 542666486 m_Children
# Find the m_Children block of 542666486
$pattern = "(--- !u!224 &542666486[\s\S]*?m_Children:)([\s\S]*?)(m_Father:)"
if ($content -match $pattern) {
    $childrenBlock = $matches[2]
    # Replace 929201233 with a temp placeholder, 1649789105 with 929201233, and placeholder with 1649789105
    $swapped = $childrenBlock.Replace("fileID: 929201233", "fileID: TEMP_SWAP_ID")
    $swapped = $swapped.Replace("fileID: 1649789105", "fileID: 929201233")
    $swapped = $swapped.Replace("fileID: TEMP_SWAP_ID", "fileID: 1649789105")
    
    $content = $content.Replace($matches[0], $matches[1] + $swapped + $matches[3])
    Write-Output "Successfully swapped children in 542666486"
} else {
    Write-Output "Could not find 542666486 m_Children"
}

[System.IO.File]::WriteAllText($path, $content)
Write-Output "Saved GameScene.unity successfully"