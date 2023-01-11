set CurDir=%~dp0
set LibName=%1
xcopy  %LibName%.dll  %CurDir%..\..\TitanSiege\Assets\Plugins\ManagedLib\ /y