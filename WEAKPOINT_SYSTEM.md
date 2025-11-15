# 약점 시스템 (Weakpoint System)

## 🎯 개요

마법진의 특정 부분을 **약점(Weakpoint)**으로 지정하여, 플레이어가 명확히 어디를 자를지 알 수 있도록 개선했습니다.

## ✨ 주요 특징

### 시각적 구분
- **약점 선분**: 빨간색 (기본), 일반 선분보다 1.5배 두껍게 표시
- **일반 선분**: 보라색 (기본)
- **Hierarchy 이름**: `Segment_N_Weakpoint` 형식으로 표시

### 게임 규칙
- **승리 조건**: 모든 약점을 파괴하면 성공
- **시간 제한**: 3초 안에 모든 약점을 끊어야 함
- **실패 조건**: 시간 초과 시 마법 발동

## 🎨 Inspector 설정

### MagicCircle Prefab
```
Settings
├── Circle Color: 일반 선분 색상 (보라색)
└── Line Width: 선 두께

Visual Feedback
├── Broken Color: 끊어진 선분 색상 (회색)
└── Weakpoint Color: 약점 색상 (빨간색)

Weakpoint Settings
└── Weakpoint Ratio: 약점 비율 (0.1~0.5)
    - 0.1 = 10% (쉬움)
    - 0.3 = 30% (보통) ← 기본값
    - 0.5 = 50% (어려움)
```

## 📊 약점 분배 방식

### 균등 분산
약점은 마법진 전체에 **균등하게 분산**되어 배치됩니다.

**예시** (Pentagram, 5개 선분):
- Weakpoint Ratio: 0.3
- 약점 개수: 5 × 0.3 = 1.5 → 2개
- 약점 위치: Segment 0, Segment 2 (균등 분산)

### 패턴별 약점 개수

| 패턴 | 전체 선분 | 약점 (30%) | 실제 약점 |
|------|-----------|------------|-----------|
| Circle | 32 | 9.6 | 10개 |
| Triangle | 3 | 0.9 | 1개 |
| Square | 4 | 1.2 | 1개 |
| Pentagram | 5 | 1.5 | 2개 |
| Hexagram | 13 | 3.9 | 4개 |
| DoublePentagram | 11 | 3.3 | 3개 |

## 🎮 플레이 방법

### 1단계: 약점 확인
- Play 버튼 클릭
- 화면에 마법진이 그려짐
- **빨간색 선분**이 약점

### 2단계: 약점 파괴
- 빨간색 선분을 마우스 드래그로 자르기
- 자른 부분은 페이드 아웃되며 사라짐
- UI에 진행도 표시: `Destroy 2/3 weakpoints!`

### 3단계: 성공/실패
- **성공**: 모든 약점 파괴 → 다음 스테이지
- **실패**: 시간 초과 → 게임 재시작

## 💡 난이도 조절

### 쉽게
- **Weakpoint Ratio**: `0.1` (10%)
- **Cast Time**: `5초`
- 약점이 적고 시간이 많음

### 보통
- **Weakpoint Ratio**: `0.3` (30%) ← 기본값
- **Cast Time**: `3초`
- 균형잡힌 난이도

### 어렵게
- **Weakpoint Ratio**: `0.5` (50%)
- **Cast Time**: `2초`
- 약점이 많고 시간이 적음

## 🔧 커스터마이징

### 약점 색상 변경
```
Inspector > MagicCircle Prefab > Weakpoint Color
```

**추천 색상:**
- **빨간색** (기본): 위험, 공격적
- **노란색**: 주의, 경고
- **주황색**: 열정, 에너지
- **하얀색**: 순수, 빛
- **금색**: 가치, 보상

### 약점 위치 하이라이팅 강화
약점을 더 눈에 띄게 만들려면:
1. **Weakpoint Color** → 더 밝은 색
2. **Line Width** → 약점이 더 두껍게 (코드 수정: `lineWidth * 2f`)
3. **Sorting Order** → 약점이 항상 위에 (이미 적용됨)

## 📝 코드 구조

### LineSegment.cs
```csharp
public bool isWeakpoint = false; // 약점 플래그
```

### MagicCircle.cs
```csharp
public int GetTotalWeakpointCount()      // 전체 약점 개수
public int GetBrokenWeakpointCount()     // 끊어진 약점 개수
```

### GameManager.cs
```csharp
// 승리 조건: 모든 약점 파괴
if (currentCircle.GetBrokenWeakpointCount() >= currentCircle.GetTotalWeakpointCount())
{
    OnPlayerWin();
}
```

## 🆚 이전 vs 현재

### 이전 (일반 선분 카운트)
- ❌ 어디를 자를지 불명확
- ❌ 아무 선분이나 3개 자르면 됨
- ❌ 전략성 부족

### 현재 (약점 시스템)
- ✅ 약점이 시각적으로 명확 (빨간색)
- ✅ 특정 부분만 자르면 됨
- ✅ 전략적 플레이 가능
- ✅ 목표가 명확함

## 🎯 POC 검증 개선

약점 시스템으로 인한 개선:
1. **직관성**: 플레이어가 즉시 이해
2. **목표 명확성**: 빨간색 = 자를 곳
3. **만족감**: 약점 파괴 시 명확한 진행도
4. **난이도 조절**: Ratio로 쉽게 조절

이제 게임이 훨씬 더 **직관적이고 게임다워졌습니다**!
