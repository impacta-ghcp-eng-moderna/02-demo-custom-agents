using System.Net;
using System.Net.Http.Json;
using TrainingCatalog.Application;

namespace TrainingCatalog.Api.Tests;

public sealed class TrainingCountTests
{
	[Fact]
	public async Task ReturnsTrainingCountAfterTrainingIsCreated()
	{
		using var factory = new TrainingCatalogApiFactory();
		using var client = factory.CreateClient();
		var request = new CreateTrainingRequest(
			"Fundamentos de C#",
			"Introdução ao C#",
			"2026-09-15",
			8);

		var creationResponse = await client.PostAsJsonAsync("/api/trainings", request);
		var countResponse = await client.GetAsync("/api/trainings/count");

		Assert.Equal(HttpStatusCode.Created, creationResponse.StatusCode);
		Assert.Equal(HttpStatusCode.OK, countResponse.StatusCode);
		var result = await countResponse.Content.ReadFromJsonAsync<TrainingCount>();
		Assert.NotNull(result);
		Assert.Equal(1, result.Count);
	}
}
