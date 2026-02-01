namespace ApiFootball.Dtos;

public record TeamBaseResponse(int? Id);
public record TeamResponse(
    int? Id,
    string Name,
    string Crest
) : TeamBaseResponse(Id);