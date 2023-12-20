namespace SpaceShipAPI.Model.DTO;

public record SpaceStationDTO(long Id, String Name, HangarDTO Hangar, SpaceStationStorageDTO Storage) {

}