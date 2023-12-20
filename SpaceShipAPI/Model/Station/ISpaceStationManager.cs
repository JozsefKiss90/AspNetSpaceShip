using System;
using System.Collections.Generic;
using SpaceShipAPI;
using SpaceShipAPI.Model.DTO;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Services;
using SpaceShipAPI.Utils;

namespace SpaceshipAPI.Spaceship.Model.Station
{
    public interface ISpaceStationManager
    {
        SpaceStation CreateNewSpaceStation(string name);
        bool RemoveResources(Dictionary<ResourceType, int> cost, SpaceStation station);
        bool AddNewShip(SpaceShip ship, ShipType shipType, SpaceStation station);
        Dictionary<ResourceType, int> GetStoredResources(SpaceStation station);
        Dictionary<ResourceType, int> GetStorageUpgradeCost(SpaceStation station);
        Dictionary<ResourceType, int> GetHangarUpgradeCost(SpaceStation station);
        bool RemoveShip(SpaceShip ship, SpaceStation station);
        HashSet<SpaceShip> GetAllShips(SpaceStation station);
        bool HasShipAvailable(SpaceShip ship, SpaceStation station);
        bool UpgradeHangar(SpaceStation station);
        bool AddResourcesFromShip(MinerShipManager shipManager, Dictionary<ResourceType, int> resources, SpaceStation station);
        bool UpgradeStorage(SpaceStation station);
        bool AddResource(ResourceType resourceType, int quantity, SpaceStation station);
        SpaceStationDTO GetStationDTO(SpaceStation station);
        SpaceStationStorageDTO GetStorageDTO(SpaceStation station);
        HangarDTO GetHangarDTO(SpaceStation station);
        void CreateStorageIfNotExists(SpaceStation station);
        HangarDTO UpdateHangarDTO(HangarDTO hangar);
        void CreateHangarIfNotExists(SpaceStation station);
    }
}