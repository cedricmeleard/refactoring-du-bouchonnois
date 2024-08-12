using Bouchonnois.Domain;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bouchonnois.Tests.Assert;

public static class PartieDeChasseExtensions
{
    public static PartieDeChasseAssertions Should(this PartieDeChasse? partieDeChasse)
    {
        return new PartieDeChasseAssertions(partieDeChasse);
    }
}

public class PartieDeChasseAssertions(PartieDeChasse? partieDeChasse) : ReferenceTypeAssertions<PartieDeChasse?, PartieDeChasseAssertions>(partieDeChasse)
{
    protected override string Identifier => "Partie de chasse";

    private AndConstraint<PartieDeChasseAssertions> Call(Action act)
    {
        act();
        return new AndConstraint<PartieDeChasseAssertions>(this);
    }

    private Chasseur Chasseur(string nom)
    {
        return Subject!.Chasseurs.First(c => c.Nom == nom);
    }

    public AndConstraint<PartieDeChasseAssertions> HaveEmittedEvent(
        DateTime expectedTime,
        string expectedMessage)
    {
        var expectedEvent = new Event(expectedTime, expectedMessage);

        return Call(
            () => Execute.Assertion
                .ForCondition(!string.IsNullOrEmpty(expectedMessage))
                .FailWith("Impossible de faire une assertion sur un message vide")
                .Then
                .Given(() => Subject!.Events)
                .ForCondition(events => events.Count == 1 && events.Last() == new Event(expectedTime, expectedMessage))
                .FailWith($"Les events devraient contenir {expectedEvent}."));
    }


    public AndConstraint<PartieDeChasseAssertions> ChasseurATiréSurUneGalinette(
        string nom,
        int ballesRestantes,
        int galinettes)
    {
        return Call(() =>
            Execute.Assertion
                .ForCondition(Subject!.Chasseurs.Any(c => c.Nom == nom))
                .FailWith("Chasseur non présent dans la partie de chasse")
                .Then
                .Given(() => Subject!.Chasseurs.First(c => c.Nom == nom))
                .ForCondition(
                    chasseur => chasseur.BallesRestantes == ballesRestantes && chasseur.NbGalinettes == galinettes)
                .FailWith(
                    $"Le nombre de balles restantes pour {nom} devrait être de {ballesRestantes} balle(s) et il devrait avoir capturé {galinettes} galinette(s), " +
                    $"il lui reste {Chasseur(nom).BallesRestantes} balle(s) et a capturé {Chasseur(nom).NbGalinettes} galinette(s)"));
    }


    public AndConstraint<PartieDeChasseAssertions> GalinettesSurLeTerrain(int nbGalinettes)
    {
        return Call(() =>
            Execute.Assertion
                .Given(() => Subject!.Terrain!)
                .ForCondition(terrain => terrain.NbGalinettes == nbGalinettes)
                .FailWith($"Le terrain devrait contenir {nbGalinettes} mais en contient {Subject!.Terrain.NbGalinettes}"));
    }

    public AndConstraint<PartieDeChasseAssertions> LaPartieEstEnCours()
    {
        return Call(() =>
            Execute.Assertion
                .Given(() => Subject!)
                .ForCondition(partie => partie.Status == PartieStatus.EnCours)
                .FailWith("La partie devrait être en cours"));
    }
    public AndConstraint<PartieDeChasseAssertions> CestLePetitApero()
    {
        return Call(() =>
            Execute.Assertion
                .Given(() => Subject!)
                .ForCondition(partie => partie.Status == PartieStatus.Apéro)
                .FailWith("Ca devrait être un petit apéro"));
    }
    public AndConstraint<PartieDeChasseAssertions> LaPartieEstTerminée()
    {
        return Call(() =>
            Execute.Assertion
                .Given(() => Subject!)
                .ForCondition(partie => partie.Status == PartieStatus.Terminée)
                .FailWith("La partie devrait être terminée"));
    }
}