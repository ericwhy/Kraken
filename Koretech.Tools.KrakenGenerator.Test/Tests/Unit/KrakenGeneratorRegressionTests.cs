namespace Koretech.Tools.KrakenGenerator.Test.Tests.Unit;

/// <summary>
/// Verifies the generator output for representative KAMLBO inputs.
/// </summary>
[TestFixture]
public class KrakenGeneratorRegressionTests
{
    /// <summary>
    /// Verifies the baseline KSUser generation contract.
    /// </summary>
    [Test]
    public void Generate_KsUser_CreatesExpectedOutputs()
    {
        using KrakenGeneratorTestHarness harness = KrakenGeneratorTestHarness.Generate("KSUser.kamlbo");

        Assert.Multiple(() =>
        {
            Assert.That(harness.GetOutputFile(@"Entities\KsUser\KsUserEntity_Gen.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Configurations\KsUserEntityTypeConfiguration.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Contexts\KsUserContext.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Repositories\KsUserRepository_Gen.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile("KsUserService_Gen.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile("IKsUserService_Gen.cs").Exists, Is.True);
            Assert.That(harness.DirectoryExists("BusinessObjects"), Is.False);
        });

        string entityFile = harness.ReadOutputFile(@"Entities\KsUser\KsUserEntity_Gen.cs");
        string repositoryFile = harness.ReadOutputFile(@"Repositories\KsUserRepository_Gen.cs");
        string serviceFile = harness.ReadOutputFile("KsUserService_Gen.cs");

        Assert.Multiple(() =>
        {
            Assert.That(entityFile, Does.Contain("public partial class KsUserEntity"));
            Assert.That(repositoryFile, Does.Contain("internal partial class KsUserRepository : Repository<KsUserEntity>"));
            Assert.That(serviceFile, Does.Contain("Task<IEnumerable<KsUserEntity>> GetAllAsync()").Or.Contain("public async Task<IEnumerable<KsUserEntity>> GetAllAsync()"));
        });
    }

    /// <summary>
    /// Verifies the EF configuration and relationship output for KSUser.
    /// </summary>
    [Test]
    public void Generate_KsUser_EmitsExpectedConfigurationAndContextDetails()
    {
        using KrakenGeneratorTestHarness harness = KrakenGeneratorTestHarness.Generate("KSUser.kamlbo");

        string configurationFile = harness.ReadOutputFile(@"Configurations\KsUserEntityTypeConfiguration.cs");
        string contextFile = harness.ReadOutputFile(@"Contexts\KsUserContext.cs");

        Assert.Multiple(() =>
        {
            Assert.That(configurationFile, Does.Contain("UseSqlOutputClause(false)"));
            Assert.That(configurationFile, Does.Not.Contain(".HasName("));
            Assert.That(contextFile, Does.Contain("modelBuilder.ApplyConfiguration(new KsUserEntityTypeConfiguration());"));
            Assert.That(contextFile, Does.Contain(".OnDelete(DeleteBehavior.Cascade);"));
            Assert.That(contextFile, Does.Not.Contain(".Ignore("));
        });
    }

    /// <summary>
    /// Verifies that composite metadata expands into split entity generation for Customer.
    /// </summary>
    [Test]
    public void Generate_Customer_CreatesCompositeEntityOutputs()
    {
        using KrakenGeneratorTestHarness harness = KrakenGeneratorTestHarness.Generate("Customer.kamlbo");

        Assert.Multiple(() =>
        {
            Assert.That(harness.GetOutputFile(@"Entities\Customer\CustomerEntity_Gen.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Entities\Customer\Customer_EntityAddressEntity_Gen.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Configurations\Customer_EntityAddressEntityTypeConfiguration.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Contexts\CustomerContext.cs").Exists, Is.True);
            Assert.That(harness.GetOutputFile(@"Repositories\CustomerRepository_Gen.cs").Exists, Is.True);
        });

        string rootEntityFile = harness.ReadOutputFile(@"Entities\Customer\CustomerEntity_Gen.cs");
        string compositeEntityFile = harness.ReadOutputFile(@"Entities\Customer\Customer_EntityAddressEntity_Gen.cs");

        Assert.Multiple(() =>
        {
            Assert.That(rootEntityFile, Does.Contain("public ICollection<Customer_EntityAddressEntity> CustomerToCustomer_EntityAddress"));
            Assert.That(compositeEntityFile, Does.Contain("public int BusinessNo { get; set; } = default!;"));
            Assert.That(compositeEntityFile, Does.Contain("public CustomerEntity? Customer_EntityAddressToCustomer { get; set; } = null;"));
        });
    }

    /// <summary>
    /// Verifies the composite-specific ORM behavior for Customer.
    /// </summary>
    [Test]
    public void Generate_Customer_EmitsCompositeQueryFilterAndRepositoryInclude()
    {
        using KrakenGeneratorTestHarness harness = KrakenGeneratorTestHarness.Generate("Customer.kamlbo");

        string contextFile = harness.ReadOutputFile(@"Contexts\CustomerContext.cs");
        string repositoryFile = harness.ReadOutputFile(@"Repositories\CustomerRepository_Gen.cs");

        Assert.Multiple(() =>
        {
            Assert.That(contextFile, Does.Contain("HasQueryFilter(e => e.PrimaryFlg == 'Y');"));
            Assert.That(contextFile, Does.Contain("Customer_EntityAddressToCustomer"));
            Assert.That(contextFile, Does.Contain(".OnDelete(DeleteBehavior.Cascade);"));
            Assert.That(contextFile, Does.Contain("modelBuilder.Entity<CustomerEntity>().Ignore(e => e.CustomerContactList);"));
            Assert.That(repositoryFile, Does.Contain(".Include(entity => entity.CustomerToCustomer_EntityAddress)"));
        });
    }
}
