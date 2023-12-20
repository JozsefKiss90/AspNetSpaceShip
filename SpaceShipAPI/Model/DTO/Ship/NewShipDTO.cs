using SpaceShipAPI.Model.Ship;

namespace SpaceShipAPI.Model.DTO.Ship;

public record NewShipDTO(String name, ShipColor color, ShipType type) {
}